using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using CSMSL.Chemistry;
using CSMSL.Proteomics;

namespace CSMSL.IO.PepXML
{
    public class PepXmlWriter : IDisposable
    {

        public string FilePath { get; private set; }

        private XmlTextWriter _writer;

        public PepXmlWriter(string filePath)
        {
            FilePath = filePath;            
            _writer = new XmlTextWriter(filePath, Encoding.UTF8);
            _writer.Formatting = Formatting.Indented;
            _writer.WriteStartDocument();
            _writer.WriteStartElement("msms_pipeline_analysis");
            _writer.WriteAttributeString("date", DateTime.Now.ToString("s"));
            _writer.WriteAttributeString("summary_xml", filePath);
            _writer.WriteAttributeString("xmlns", "http://regis-web.systembiology.net/pepXML");
        }

        public void WriteModification(IMass modification, ModificationSites sites, bool fixedModification = true)
        {
            foreach (ModificationSites singleSite in Modification.GetSites(sites))
            {
                double basemass = 0;
                if (singleSite >= ModificationSites.NPep)
                {
                    _writer.WriteStartElement("terminal_modification");
                    if (singleSite.HasFlag(ModificationSites.NPep) || singleSite.HasFlag(ModificationSites.NProt))
                    {
                        _writer.WriteAttributeString("terminus", "N");
                        basemass += AminoAcidPolymer.DefaultNTerminus.MonoisotopicMass;
                    }
                    else
                    {
                        _writer.WriteAttributeString("terminus", "C");
                        basemass += AminoAcidPolymer.DefaultCTerminus.MonoisotopicMass;
                    }
                   
                    _writer.WriteAttributeString("protein_terminus", (singleSite.HasFlag(ModificationSites.NProt) || singleSite.HasFlag(ModificationSites.ProtC)) ? "Y" : "N");
                }
                else
                {
                    string letter = Enum.GetName(typeof(ModificationSites), singleSite);
                    AminoAcid aa = AminoAcid.GetResidue(letter);
                    _writer.WriteStartElement("aminoacid_modification");
                    _writer.WriteAttributeString("aminoacid", letter);
                    basemass += aa.MonoisotopicMass;

                }
                double massshift = modification.MonoisotopicMass;
                _writer.WriteAttributeString("variable", (fixedModification) ? "N" : "Y");
                _writer.WriteAttributeString("mass", (basemass + massshift).ToString());
                _writer.WriteAttributeString("massdiff", massshift.ToString());
                _writer.WriteAttributeString("description", modification.ToString());
                _writer.WriteEndElement();
            }
        }

        public void WriteProtease(Protease protease)
        {
            _writer.WriteStartElement("sample_enzyme");
            _writer.WriteAttributeString("name", protease.Name);
            _writer.WriteStartElement("specificity");
            _writer.WriteAttributeString("sense", Enum.GetName(typeof(Terminus), protease.Terminal));
            _writer.WriteAttributeString("pattern", protease.CleavagePattern);
            _writer.WriteAttributeString("cut", protease.Cut);
            _writer.WriteAttributeString("no_cut", protease.NoCut);
            _writer.WriteEndElement(); // specificity
            _writer.WriteEndElement(); // sample_enzyme
        }

        public void Dispose()
        {
            if (_writer != null)
            {
                _writer.WriteEndElement(); // msms_pepeline_anlaysis;
                _writer.WriteEndDocument();
                _writer.Dispose();
            }
        }
    }
}
