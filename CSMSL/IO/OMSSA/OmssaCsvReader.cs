using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using CsvHelper;
using CSMSL.Proteomics;
using CSMSL.Analysis.Identification;

namespace CSMSL.IO.OMSSA
{
    public class OmssaCsvReader : IDisposable
    {
        public string FilePath { get; private set; }

        private CsvReader _reader;

        public OmssaCsvReader()
        {
                       
        }


        private Dictionary<string, Protein> _proteins;

        public void LoadProteins(IEnumerable<Protein> proteins)
        {
            _proteins = new Dictionary<string, Protein>();
            foreach (Protein prot in proteins)
            {
                _proteins.Add(prot.Description, prot);
            }
        }

        public void LoadProteins(string fastaFile)
        {
            using (FastaReader reader = new FastaReader(fastaFile))
            {
                LoadProteins(reader.ReadNextProtein());
            }
        }
        
        private void SetMods(AminoAcidPolymer peptide, string modLine)
        {

        }

        public IEnumerable<OmssaPeptideSpectralMatch> ReadFrom(string filePath) {
            _reader = new CsvReader(new StreamReader(filePath));
            return _reader.GetRecords<OmssaPeptideSpectralMatch>();
        }

        public IEnumerable<PeptideSpectralMatch> ReadPsmsFrom(string filePath)
        {
            Protein prot;            
            foreach (OmssaPeptideSpectralMatch omssaPSM in ReadFrom(filePath))
            {
                Peptide peptide = new Peptide(omssaPSM.Sequence.ToUpper());
                SetMods(peptide, omssaPSM.Modifications);
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
                yield return psm;
            }
        }

        private bool _disposed;

        protected virtual void Dispose(bool disposing)
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
            if (_proteins != null)
                _proteins.Clear();
            _proteins = null;
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
