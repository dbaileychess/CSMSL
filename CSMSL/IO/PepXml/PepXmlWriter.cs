// Copyright 2012, 2013, 2014 Derek J. Bailey
// 
// This file (PepXmlWriter.cs) is part of CSMSL.
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
using System.Text;
using System.Xml;
using System.IO;
using CSMSL.Chemistry;
using CSMSL.Proteomics;
using CSMSL.Analysis.Identification;

namespace CSMSL.IO.PepXML
{
    public class PepXmlWriter : IDisposable
    {
        public enum Stage
        {
            RunSummary,
            SearchSummary,
            Spectra
        }

        public string FilePath { get; private set; }
        public string BaseName { get; private set; }
        public Stage CurrentStage { get; private set; }

        private int _spectrumIndex = 1;

        private XmlTextWriter _writer;

        public PepXmlWriter(string filePath)
        {
            FilePath = filePath;
            BaseName = Path.GetFileNameWithoutExtension(filePath);

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
                case Stage.Spectra:
                default:
                    break;
            }
        }

        public void WriteParameter(string name, string value)
        {
            _writer.WriteStartElement("parameter");
            _writer.WriteAttributeString("name", name);
            _writer.WriteAttributeString("value", value);
            _writer.WriteEndElement(); // parameter          
        }

        public void WriteComment(string comment)
        {
            _writer.WriteComment(comment);
        }

        private double spectrumNeutralMass;

        public void StartSpectrum(int spectrumNumbner, double rt, double precursorNeutralMass, int chargeState, string title = "")
        {
            _writer.WriteStartElement("spectrum_query");
            _writer.WriteAttributeString("spectrum", title);
            _writer.WriteAttributeString("start_scan", spectrumNumbner.ToString());
            _writer.WriteAttributeString("end_scan", spectrumNumbner.ToString());
            _writer.WriteAttributeString("precursor_neutral_mass", precursorNeutralMass.ToString());
            _writer.WriteAttributeString("assumed_charge", chargeState.ToString());
            _writer.WriteAttributeString("index", _spectrumIndex.ToString());
            _writer.WriteAttributeString("retention_time_min", rt.ToString());
            _writer.WriteStartElement("search_result");
            spectrumNeutralMass = precursorNeutralMass;
        }

        public void WritePSM(PeptideSpectralMatch psm, int hitRank = 1)
        {
            _writer.WriteStartElement("search_hit");
            _writer.WriteAttributeString("hit_rank", hitRank.ToString());
            _writer.WriteAttributeString("peptide", psm.Peptide.Sequence);
            _writer.WriteAttributeString("peptide_prev_aa", (psm.Peptide.PreviousAminoAcid != null) ? psm.Peptide.PreviousAminoAcid.Letter.ToString() : "-");
            _writer.WriteAttributeString("peptide_next_aa", (psm.Peptide.NextAminoAcid != null) ? psm.Peptide.NextAminoAcid.Letter.ToString() : "-");

            double pepMonoMass = psm.Peptide.MonoisotopicMass;
            double massDifference = spectrumNeutralMass - pepMonoMass;
            _writer.WriteAttributeString("calc_neutral_pep_mass", pepMonoMass.ToString());
            _writer.WriteAttributeString("massdiff", massDifference.ToString());

            Protein protein = psm.Peptide.Parent as Protein;

            if (protein != null)
            {
                _writer.WriteAttributeString("protein", protein.Description);
                _writer.WriteAttributeString("protein_descr", protein.Description);
            }

            _writer.WriteAttributeString("num_tot_proteins", "1");
            _writer.WriteAttributeString("is_rejected", "0");

            _writer.WriteStartElement("search_score");
            _writer.WriteAttributeString("name", Enum.GetName(typeof (PeptideSpectralMatchScoreType), psm.ScoreType));
            _writer.WriteAttributeString("value", psm.Score.ToString());
            _writer.WriteEndElement(); // search_score

            _writer.WriteEndElement(); // search_hit
        }

        public void EndSpectrum()
        {
            _spectrumIndex++;
            _writer.WriteEndElement(); // search_result
            _writer.WriteEndElement(); // spectrum_query
        }

        public void StartSearchSummary(string searchEngine, bool isPrecursorMassMonoisotopic, bool isProductMassMonoisotopic)
        {
            SetCurrentStage(Stage.SearchSummary, false);

            _writer.WriteAttributeString("base_name", BaseName);
            _writer.WriteAttributeString("search_engine", searchEngine);
            _writer.WriteAttributeString("precursor_mass_type", (isPrecursorMassMonoisotopic) ? "monoisotopic" : "average");
            _writer.WriteAttributeString("fragment_mass_type", (isProductMassMonoisotopic) ? "monoisotopic" : "average");
            _writer.WriteAttributeString("search_id", "1");
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

            foreach (ModificationSites singleSite in sites.GetActiveSites())
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
                    string letter = Enum.GetName(typeof (ModificationSites), singleSite);
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

        public void WriteSearchProtease(Protease protease, int maxMissedClevages, bool semiDigested = false)
        {
            if (CurrentStage != Stage.SearchSummary)
                throw new ArgumentException("You must be in the Search Summary stage to write modifications");

            _writer.WriteStartElement("enzymatic_search_constraint");
            _writer.WriteAttributeString("enzyme", protease.Name);
            _writer.WriteAttributeString("max_num_internal_clevages", maxMissedClevages.ToString());
            _writer.WriteAttributeString("min_number_termini", (semiDigested) ? "1" : "2");
            _writer.WriteEndElement();
        }

        public void WriteSampleProtease(Protease protease)
        {
            if (CurrentStage != Stage.RunSummary)
                throw new ArgumentException("You must be in the Run Summary stage to write proteases");

            _writer.WriteStartElement("sample_enzyme");
            _writer.WriteAttributeString("name", protease.Name);
            _writer.WriteStartElement("specificity");
            _writer.WriteAttributeString("sense", Enum.GetName(typeof (Terminus), protease.Terminal));
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