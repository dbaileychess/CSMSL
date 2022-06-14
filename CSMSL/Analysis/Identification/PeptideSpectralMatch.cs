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