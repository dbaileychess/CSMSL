// Copyright 2012, 2013, 2014 Derek J. Bailey
// 
// This file (IsobaricTag.cs) is part of CSMSL.
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