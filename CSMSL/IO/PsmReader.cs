using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMSL.Proteomics;
using CSMSL.Analysis.Identification;
using CSMSL.Chemistry;

namespace CSMSL.IO
{
    public abstract class PsmReader : IDisposable
    {
        public string FilePath { get; private set; }

        public PsmReader(string filePath)
        {
            FilePath = filePath;
            _proteins = new Dictionary<string, Protein>();
            _fixedMods = new List<Tuple<IChemicalFormula, ModificationSites>>();
        }
                       
        protected Dictionary<string, Protein> _proteins;

        protected List<Tuple<IChemicalFormula, ModificationSites>> _fixedMods;

        public abstract IEnumerable<PeptideSpectralMatch> ReadNextPsm();

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
          
        public void AddFixedModification(string chemicalFormula, ModificationSites sites)
        {
            AddFixedModification(new ChemicalFormula(chemicalFormula), sites);
        }

        public void AddFixedModification(IChemicalFormula modification, ModificationSites sites)
        {
            _fixedMods.Add(new Tuple<IChemicalFormula, ModificationSites>(modification, sites));
        }

        protected virtual void SetFixedMods(AminoAcidPolymer peptide)
        {
            foreach (Tuple<IChemicalFormula, ModificationSites> mods in _fixedMods)
            {
                peptide.SetModification(mods.Item1, mods.Item2);
            }
        }

        protected bool _disposed;

        protected virtual void Dispose(bool disposing)
        {            
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
