﻿// Copyright 2022 Derek J. Bailey
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
using CSMSL.Chemistry;

namespace CSMSL.Proteomics
{
    public class ChemicalFormulaModification : Modification, IChemicalFormula
    {
        /// <summary>
        /// The Chemical Formula of this modifications
        /// </summary>
        public ChemicalFormula ChemicalFormula { get; private set; }

        public ChemicalFormulaModification(string chemicalFormula, ModificationSites sites = ModificationSites.Any)
            : this(new ChemicalFormula(chemicalFormula), "", sites)
        {
            Name = ChemicalFormula.ToString();
        }

        public ChemicalFormulaModification(string chemicalFormula, string name, ModificationSites sites = ModificationSites.Any)
            : this(new ChemicalFormula(chemicalFormula), name, sites)
        {
        }

        public ChemicalFormulaModification(ChemicalFormula chemicalFormula, string name, ModificationSites sites = ModificationSites.Any)
            : base(chemicalFormula.MonoisotopicMass, name, sites)
        {
            ChemicalFormula = chemicalFormula;
        }

        public ChemicalFormulaModification(ChemicalFormulaModification other)
            : this(new ChemicalFormula(other.ChemicalFormula), other.Name, other.Sites)
        {
        }
    }
}