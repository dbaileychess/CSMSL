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

using CSMSL.Analysis.Identification;
using CSMSL.Chemistry;
using CSMSL.Proteomics;
using System;
using System.Collections.Generic;
using CSMSL.Spectral;

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
            _dataFiles = new Dictionary<string, MSDataFile<ISpectrum>>();
            _extraColumns = new List<string>();
        }

        protected Dictionary<string, Protein> _proteins;

        protected List<Modification> _fixedMods;
        protected Dictionary<string, IMass> _variableMods;
        protected Dictionary<string, MSDataFile<ISpectrum>> _dataFiles;

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

        public void AddMSDataFiles(IEnumerable<MSDataFile<ISpectrum>> dataFiles)
        {
            foreach (MSDataFile<ISpectrum> dataFile in dataFiles)
            {
                AddMSDataFile(dataFile);
            }
        }

        public void AddMSDataFile(MSDataFile<ISpectrum> dataFile)
        {
            _dataFiles.Add(dataFile.Name, dataFile);
        }

        public void AddVariableModification(string chemicalFormula, string name)
        {
            AddVariableModification((IMass) new ChemicalFormula(chemicalFormula), name);
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