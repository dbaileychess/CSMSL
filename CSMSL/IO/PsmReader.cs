using CSMSL.Analysis.Identification;
using CSMSL.Chemistry;
using CSMSL.Proteomics;
using System;
using System.Collections.Generic;

namespace CSMSL.IO
{
    public abstract class PsmReader : IDisposable
    {
        public string FilePath { get; private set; }

        protected PsmReader(string filePath)
        {
            FilePath = filePath;
            _proteins = new Dictionary<string, Protein>();
            _fixedMods = new List<Modification>();
            _variableMods = new Dictionary<string, IMass>();
            _dataFiles = new Dictionary<string, MSDataFile>();
            _extraColumns = new List<string>();
        }
                       
        protected Dictionary<string, Protein> _proteins;

        protected List<Modification> _fixedMods;
        protected Dictionary<string, IMass> _variableMods;
        protected Dictionary<string, MSDataFile> _dataFiles;

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

        public void AddMSDataFiles(IEnumerable<MSDataFile> dataFiles)
        {
            foreach (MSDataFile dataFile in dataFiles)
            {
                AddMSDataFile(dataFile);
            }
        }

        public void AddMSDataFile(MSDataFile dataFile)
        {
            _dataFiles.Add(dataFile.Name, dataFile);          
        }

        public void AddVariableModification(string chemicalFormula, string name)
        {
            AddVariableModification(new ChemicalFormula(chemicalFormula), name);
        }

        public void AddVariableModification(IMass modification, string name)
        {
            _variableMods.Add(name, modification);
        }
    
        public IMass AddFixedModification(Modification modification)
        {
            _fixedMods.Add(modification);
            return modification;
        }

        public virtual void AddFixedModifications(IEnumerable<Modification> modifications)
        {
            _fixedMods.AddRange(modifications);
        }

        protected virtual void SetFixedMods(AminoAcidPolymer peptide)
        {
            foreach (Modification mod in _fixedMods)
            {
                peptide.SetModification(mod);
            }
        }

        protected virtual void SetVariableMods(AminoAcidPolymer peptide, string modname, int residue)
        {
            IMass mod;
            if (_variableMods.TryGetValue(modname, out mod))
            {
                peptide.SetModification(mod, residue);
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

        protected List<string> _extraColumns;

        public void ReadExtra(string p)
        {
            _extraColumns.Add(p);
        }
    }
}
