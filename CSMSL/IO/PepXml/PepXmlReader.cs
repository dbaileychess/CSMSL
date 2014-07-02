// Copyright 2012, 2013, 2014 Derek J. Bailey
// 
// This file (PepXmlReader.cs) is part of CSMSL.
// 
// CSMSL is free software: you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// CSMSL is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public
// License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with CSMSL. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using CSMSL.Proteomics;

namespace CSMSL.IO.PepXML
{
    public class PepXmlReader : IDisposable
    {
        public string FilePath { get; private set; }
        private XmlDocument _document;
        private XmlNode _root;
        private XmlNamespaceManager _nsmgr;
        
        public PepXmlReader(string filePath)
        {
            FilePath = filePath;
            _document = new XmlDocument();
            _document.Load(filePath);
            _root = _document.DocumentElement;
            _nsmgr = new XmlNamespaceManager(_document.NameTable);
            _nsmgr.AddNamespace("pepxml", "http://regis-web.systembiology.net/pepXML");
        }

        public Protease GetSampleProtease()
        {
            var node = _root.SelectSingleNode("//pepxml:sample_enzyme", _nsmgr);
            var name = node.Attributes["name"].Value;
            Protease protease;

            var specificityNode = node.FirstChild;
            string sense = specificityNode.Attributes["sense"].Value;
            string cut = specificityNode.Attributes["cut"].Value;
            string nocut = specificityNode.Attributes["no_cut"].Value;
            protease = new Protease(name, (sense.Equals("C")) ? Terminus.C : Terminus.N, cut, nocut);


            //Protease protease = new Protease(name, (sense.Equals("C")) ? Terminus.C : Terminus.N, cut, nocut);

            return protease;
        }

        public List<Modification> GetVariableModifications()
        {
            return GetModifications(false);
        }

        public List<Modification> GetFixedModifications()
        {
            return GetModifications(true);
        }

        private List<Modification> GetModifications(bool fixedMod)
        {
            var modifications = new Dictionary<string, Modification>();
            var node = _root.SelectSingleNode("//pepxml:search_summary", _nsmgr);
            XmlNodeList nodes;
            if (fixedMod)
            {
                nodes = node.SelectNodes("//pepxml:aminoacid_modification[@variable='N'] | //pepxml:terminal_modification[@variable='N']", _nsmgr);
            }
            else
            {
                nodes = node.SelectNodes("//pepxml:aminoacid_modification[@variable='Y'] | //pepxml:terminal_modification[@variable='Y']", _nsmgr);
            }
            foreach (XmlNode modNode in nodes)
            {
                string name = modNode.Attributes["description"].Value;
                Modification mod = null;
                if (!modifications.TryGetValue(name, out mod))
                {
                    double mass = double.Parse(modNode.Attributes["massdiff"].Value);
                    mod = new Modification(mass, name);
                    modifications.Add(name, mod);
                }

                if (modNode.Name == "aminoacid_modification")
                {
                    char position = modNode.Attributes["aminoacid"].Value[0];
                    mod.Sites = mod.Sites.Set(position);
                }
                else
                {
                    string terminus = modNode.Attributes["terminus"].Value;
                    bool proteinOnly = modNode.Attributes["protein_terminus"].Value == "Y";
                    if (terminus == "N")
                    {
                        if (proteinOnly)
                        {
                            mod.Sites |= ModificationSites.NProt;
                        }
                        else
                        {
                            mod.Sites |= ModificationSites.NPep;
                        }
                    }
                    else
                    {
                        if (proteinOnly)
                        {
                            mod.Sites |= ModificationSites.ProtC;
                        }
                        else
                        {
                            mod.Sites |= ModificationSites.PepC;
                        }
                    }
                }
            }

            return modifications.Values.ToList();
        }

        public void Dispose()
        {
        }
    }
}