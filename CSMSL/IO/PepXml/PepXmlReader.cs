// Copyright 2022 Derek J. Bailey
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
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