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

using CSMSL.Chemistry;
using CSMSL.Proteomics;

namespace CSMSL.Analysis.Quantitation
{
    public sealed class IsobaricTag : ChemicalFormulaModification, IQuantitationChannel
    {
        private readonly ChemicalFormula _reporterFormula;

        private readonly ChemicalFormula _balanceFormula;

        public ChemicalFormula totalFormula
        {
            get { return _reporterFormula + _balanceFormula; }
        }

        public double reporterMass
        {
            get { return _reporterFormula.MonoisotopicMass; }
        }

        public IsobaricTag(string chemicalFormula, string name)
            : base(chemicalFormula, name)
        {
        }

        public IsobaricTag(ChemicalFormula chemicalFormula, string name)
            : base(chemicalFormula, name)
        {
        }

        public IsobaricTag(string reporterFormula, string balanceFormula, string name)
            : base(reporterFormula + balanceFormula, name)
        {
            _reporterFormula = new ChemicalFormula(reporterFormula);
            _balanceFormula = new ChemicalFormula(balanceFormula);
        }

        public bool IsMS1Based
        {
            get { return false; }
        }

        public bool IsSequenceDependent
        {
            get { return false; }
        }

        double IQuantitationChannel.ReporterMass
        {
            get { return _reporterFormula.MonoisotopicMass; }
        }
    }
}