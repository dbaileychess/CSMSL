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
        public enum Stage
        {
            RunSummary,
            SearchSummary
        }

        public string FilePath { get; private set; }
        public Stage CurrentStage { get; private set; }

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

            SetCurrentStage(Stage.RunSummary, false);    
        }

        public void SetCurrentStage(Stage stage, bool endPreviousStage = true)
        {
            if (endPreviousStage)
                _writer.WriteEndElement();
            CurrentStage = stage;
            switch (CurrentStage)
            {
                case Stage.RunSummary:
                    _writer.WriteStartElement("msms_run_summary");
                    break;
                case Stage.SearchSummary:
                    _writer.WriteStartElement("search_summary");
                    break;
                default:
                    break;
            }
        }

        public void WriteProteinDatabase(string fastaFilePath, string name = "", string releaseDate = "")
        {
            if (CurrentStage != Stage.SearchSummary)
                throw new ArgumentException("You must be in the Search Summary stage to write protein databases");

            _writer.WriteStartElement("search_database");
            _writer.WriteAttributeString("seq_type", "AA");
            _writer.WriteAttributeString("local_path", fastaFilePath);          

            name = (string.IsNullOrEmpty(name)) ? Path.GetFileNameWithoutExtension(fastaFilePath) : name;           
            _writer.WriteAttributeString("database_name", name);

            if (!string.IsNullOrEmpty(releaseDate))
                _writer.WriteAttributeString("database_release_date", releaseDate);
            
            int entries = FastaReader.NumberOfEntries(fastaFilePath);
            _writer.WriteAttributeString("size_in_db_entries", entries.ToString());

            _writer.WriteEndElement(); // search_database
        }
           
        public void WriteModification(IMass modification, ModificationSites sites, bool fixedModification = true)
        {
            if (CurrentStage != Stage.SearchSummary)
                throw new ArgumentException("You must be in the Search Summary stage to write modifications");

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
            if (CurrentStage != Stage.RunSummary)
                throw new ArgumentException("You must be in the Run Summary stage to write proteases");

            _writer.WriteStartElement("sample_enzyme");
            _writer.WriteAttributeString("name", protease.Name);
            _writer.WriteStartElement("specificity");
            _writer.WriteAttributeString("sense", Enum.GetName(typeof(Terminus), protease.Terminal));
            //_writer.WriteAttributeString("pattern", protease.CleavagePattern); // not part of the schema
            _writer.WriteAttributeString("cut", protease.Cut);
            _writer.WriteAttributeString("no_cut", protease.NoCut);
            _writer.WriteEndElement(); // specificity
            _writer.WriteEndElement(); // sample_enzyme
        }

        public void Dispose()
        {
            if (_writer != null)
            {
                _writer.WriteEndElement(); // msms_run_summary;
                _writer.WriteEndElement(); // msms_pepeline_anlaysis;
                _writer.WriteEndDocument();
                _writer.Dispose();
            }
        }
    }
}
