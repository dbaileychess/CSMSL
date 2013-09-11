using CSMSL.Analysis.Identification;
using CSMSL.Chemistry;
using CSMSL.Proteomics;
using CSMSL.Spectral;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;

namespace CSMSL.IO.OMSSA
{
    public class OmssaCsvPsmReader : PsmReader
    {      
        private CsvReader _reader;

        private string _userModFile;
   
        public OmssaCsvPsmReader(string filePath, string userModFile = "")
            : base(filePath)
        {
            _reader = new CsvReader(new StreamReader(filePath));            
           
            if (!string.IsNullOrEmpty(userModFile))
            {
                OmssaModification.LoadOmssaModifications(userModFile, true);
            }

            _userModFile = userModFile;
            _reader.Configuration.RegisterClassMap<OmssaPeptideSpectralMatch>();
        }

        public IMass AddFixedModification(int modID, ModificationSites sites)
        {
            OmssaModification modification = null;
            if (OmssaModification.TryGetModification(modID, out modification))
            {
                _fixedMods.Add(modification);
            }
            return modification;
        }

        private void SetDynamicMods(AminoAcidPolymer peptide, string modifications)
        {
            if (peptide == null || string.IsNullOrEmpty(modifications))
                return;
            
            foreach (string modification in modifications.Split(',',';'))
            {
                string[] modParts = modification.Trim().Split(':');
                int location = 0;               

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
                if (_extraColumns.Count > 0)
                {
                    foreach(string name in _extraColumns) {
                        psm.AddExtraData(name, _reader.GetField<string>(name));
                    }                   
                }
                psm.Peptide = peptide;
                psm.Score = omssaPSM.EValue;
                psm.Charge = omssaPSM.Charge;
                psm.ScoreType = PeptideSpectralMatchScoreType.EValue;
                psm.IsDecoy = omssaPSM.Defline.StartsWith("DECOY");
                psm.SpectrumNumber = omssaPSM.SpectrumNumber;
                psm.FileName = omssaPSM.FileName;               

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
  