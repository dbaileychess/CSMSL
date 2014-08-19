// Copyright 2012, 2013, 2014 Derek J. Bailey
// 
// This file (PeptideSpectralMatch.cs) is part of CSMSL.
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

using System.ComponentModel;
using CSMSL.Chemistry;
using CSMSL.Proteomics;
using CSMSL.Spectral;
using System;
using System.Collections.Generic;

namespace CSMSL.Analysis.Identification
{
    public class PeptideSpectralMatch : IFalseDiscovery<double>, IMassSpectrum, IComparable<PeptideSpectralMatch>, IMass
    {
        public virtual Peptide Peptide { get; set; }

        public virtual MsnDataScan<ISpectrum> Scan { get; set; }

        public virtual int Charge { get; set; }

        public virtual int SpectrumNumber { get; set; }

        public virtual string FileName { get; set; }

        /// <summary>
        /// Theoretical Precursor M/Z
        /// </summary>
        public virtual double PrecursorMz
        {
            get { return Peptide.ToMz(Charge); }
        }

        public virtual double MonoisotopicMass
        {
            get { return Peptide.MonoisotopicMass; }
        }

        public virtual double IsolationMz { get; set; }

        public virtual int IsotopeSelected { get; set; }

        public virtual double AdjustedIsolationMass { get; set; }

        public virtual Tolerance PrecursorMassError { get; set; }

        public virtual Tolerance CorrectedPrecursorMassError { get; set; }

        double IMass.MonoisotopicMass
        {
            get { return MonoisotopicMass; }
        }

        public double QValue { get; set; }

        private Dictionary<string, string> _extraData;

        public PeptideSpectralMatch(PeptideSpectralMatchScoreType type = PeptideSpectralMatchScoreType.LowerIsBetter)
        {
            ScoreType = type;
        }

        /// <summary>
        /// The score of the match between the peptide and spectrum.
        /// </summary>
        public virtual double Score { get; set; }

        public PeptideSpectralMatchScoreType ScoreType { get; set; }

        public virtual bool IsDecoy { get; set; }

        public virtual double FdrScoreMetric
        {
            get { return Score; }
        }

        public ISpectrum MassSpectrum
        {
            get
            {
                if (Scan == null)
                    return null;
                return Scan.MassSpectrum;
            }
        }

        public override string ToString()
        {
            return string.Format("{0} (SN: {1} Score: {2:G3} {3})", Peptide, SpectrumNumber, Score,
                Enum.GetName(typeof (PeptideSpectralMatchScoreType), ScoreType));
        }


        /// <summary>
        /// Compares two PSM based on their score.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(PeptideSpectralMatch other)
        {
            if (other == null)
                return 1;

            if (!ScoreType.Equals(other.ScoreType))
            {
                throw new ArgumentException("Cannot compare peptide spectral matches with different score types");
            }

            // The sign of the scoretype enum indicates how they should be compared
            return Score.CompareTo(other.Score)*Math.Sign((int) ScoreType);
        }

        public string this[string name]
        {
            get { return _extraData[name]; }
        }

        public void AddExtraData(string name, string value)
        {
            if (_extraData == null)
                _extraData = new Dictionary<string, string>();
            _extraData[name] = value;
        }
    }
}