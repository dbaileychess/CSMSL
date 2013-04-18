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
using CSMSL.Spectral;
using System.Xml;

namespace CSMSL.IO.OMSSA
{
    public class OmssaCsvPsmReader : PsmReader
    {      
        private CsvReader _reader;

        private string _userModFile;

        private static Dictionary<string, IMass> _dynamicMods;
        private Dictionary<string, IMass> _userDynamicMods;

        static OmssaCsvPsmReader()
        {
            _dynamicMods = LoadMods("Resources/mods.xml");            
        }

        public OmssaCsvPsmReader(string filePath, string userModFile = "")
            : base(filePath)
        {
            _reader = new CsvReader(new StreamReader(filePath));            
           
            if (!string.IsNullOrEmpty(userModFile))
            {
                _userDynamicMods = LoadMods(userModFile);
            }
            _userModFile = userModFile;
        }

        private static Dictionary<string, IMass> LoadMods(string file)
        {
            XmlDocument mods_xml = new XmlDocument();
            mods_xml.Load(file);
            XmlNamespaceManager mods_xml_ns = new XmlNamespaceManager(mods_xml.NameTable);
            mods_xml_ns.AddNamespace("omssa", mods_xml.ChildNodes[1].Attributes["xmlns"].Value);
            Dictionary<string, IMass> mods = new Dictionary<string, IMass>();
            foreach (XmlNode mod_node in mods_xml.SelectNodes("/omssa:MSModSpecSet/omssa:MSModSpec", mods_xml_ns))
            {
                string name = mod_node.SelectSingleNode("./omssa:MSModSpec_name", mods_xml_ns).FirstChild.Value;
                double mono = double.Parse(mod_node.SelectSingleNode("./omssa:MSModSpec_monomass", mods_xml_ns).FirstChild.Value);
                double average = double.Parse(mod_node.SelectSingleNode("./omssa:MSModSpec_averagemass", mods_xml_ns).FirstChild.Value);
                OmssaModification mod = new OmssaModification(name, mono, average);
                mods.Add(name, mod);
            }
            return mods;
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

                if (int.TryParse(modParts[1], out location))
                {
                    SetVariableMods(peptide, modParts[0], location);

                    //if ((_userDynamicMods != null && _userDynamicMods.TryGetValue(modParts[0], out mod)) || _dynamicMods.TryGetValue(modParts[0], out mod))
                    //{
                    //    peptide.SetModification(mod, location);
                    //}
                    //else
                    //{
                    //    throw new ArgumentException("Could not find the following OMSSA modification: " + modParts[0]);
                    //}
                }
                else
                {
                    throw new ArgumentException("Could not parse the residue position for the following OMSSA modification: " + modification);
                }
            }
        }

        public override IEnumerable<PeptideSpectralMatch> ReadNextPsm()
        {
            Protein prot;
            MSDataFile dataFile;
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
                PeptideSpectralMatch psm = new PeptideSpectralMatch();
                psm.Peptide = peptide;
                psm.Score = omssaPSM.EValue;
                psm.ScoreType = PeptideSpectralMatchScoreType.EValue;
                psm.IsDecoy = omssaPSM.Defline.StartsWith("DECOY");
                psm.SpectrumNumber = omssaPSM.SpectrumNumber;
                psm.FileName = omssaPSM.FileName;
                psm.Charge = omssaPSM.Charge;

                string[] filenameparts = psm.FileName.Split('.');
                if (_dataFiles.TryGetValue(filenameparts[0], out dataFile))
                {
                    if (!dataFile.IsOpen)
                        dataFile.Open();
                    psm.Spectrum = dataFile[psm.SpectrumNumber] as MsnDataScan;
                }

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
