// Copyright 2012, 2013, 2014 Derek J. Bailey
// 
// This file (PsmReader.cs) is part of CSMSL.
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