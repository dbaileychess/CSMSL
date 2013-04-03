using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using CsvHelper;
using CSMSL.Proteomics;
using CSMSL.Analysis.Identification;
using CSMSL.IO;
using CSMSL.Chemistry;
using System.Xml;

namespace CSMSL.IO.OMSSA
{
    public class OmssaCsvPsmReader : PsmReader
    {      
        private CsvReader _reader;

        private string _userModFile;

        private Dictionary<string, IMass> _dynamicMods;

        public OmssaCsvPsmReader(string filePath, string userModFile = "")
            : base(filePath)
        {
            _reader = new CsvReader(new StreamReader(filePath));
            _dynamicMods = new Dictionary<string, IMass>();
            LoadDynamicMods("Resources/mods.xml");
            if (!string.IsNullOrEmpty(userModFile))
            {
                LoadDynamicMods(userModFile);
            }
            _userModFile = userModFile;
        }

        private void LoadDynamicMods(string file)
        {
            XmlDocument mods_xml = new XmlDocument();
            mods_xml.Load(file);
            XmlNamespaceManager mods_xml_ns = new XmlNamespaceManager(mods_xml.NameTable);
            mods_xml_ns.AddNamespace("omssa", mods_xml.ChildNodes[1].Attributes["xmlns"].Value);

            foreach (XmlNode mod_node in mods_xml.SelectNodes("/omssa:MSModSpecSet/omssa:MSModSpec", mods_xml_ns))
            {
                string name = mod_node.SelectSingleNode("./omssa:MSModSpec_name", mods_xml_ns).FirstChild.Value;
                double mono = double.Parse(mod_node.SelectSingleNode("./omssa:MSModSpec_monomass", mods_xml_ns).FirstChild.Value);
                double average = double.Parse(mod_node.SelectSingleNode("./omssa:MSModSpec_averagemass", mods_xml_ns).FirstChild.Value);
                Mass mass = new Mass(mono, average);
                _dynamicMods.Add(name, mass);
            }
        }

        private void SetDynamicMods(AminoAcidPolymer peptide, string modifications)
        {
            if (peptide == null || string.IsNullOrEmpty(modifications))
                return;

            foreach (string modification in modifications.Split(','))
            {
                string[] modParts = modification.Trim().Split(':');
                int location = 0;
                IMass mod = null;
                if (int.TryParse(modParts[1], out location) && _dynamicMods.TryGetValue(modParts[0], out mod))
                {
                    peptide.SetModification(mod, location);
                }
            }
        }

        public override IEnumerable<PeptideSpectralMatch> ReadNextPsm()
        {
            Protein prot;
            foreach (OmssaPeptideSpectralMatch omssaPSM in _reader.GetRecords<OmssaPeptideSpectralMatch>())
            {
                Peptide peptide = new Peptide(omssaPSM.Sequence.ToUpper());
                SetFixedMods(peptide);
                SetDynamicMods(peptide, omssaPSM.Modifications);
                peptide.StartResidue = omssaPSM.StartResidue;
                peptide.EndResidue = omssaPSM.StopResidue;
                if (_proteins.TryGetValue(omssaPSM.Defline, out prot))
                {
                    peptide.Parent = prot;
                }
                else
                {

                }
                PeptideSpectralMatch psm = new PeptideSpectralMatch();
                psm.Peptide = peptide;
                psm.Score = omssaPSM.EValue;
                psm.ScoreType = PeptideSpectralMatchScoreType.EValue;
                psm.IsDecoy = omssaPSM.Defline.StartsWith("DECOY");
                psm.SpectrumNumber = omssaPSM.SpectrumNumber;
                psm.FileName = omssaPSM.FileName;
                yield return psm;
            }
        }          

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_reader != null)
                        _reader.Dispose();
                    _reader = null;
                }
            }
            base.Dispose(disposing);
        }

   
    }
}
