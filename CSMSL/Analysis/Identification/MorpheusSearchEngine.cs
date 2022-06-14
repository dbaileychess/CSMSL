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
using CSMSL.Proteomics;
using CSMSL.Spectral;
using CSMSL.Util.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CSMSL.Analysis.Identification
{
    /// <summary>
    /// High-resolution Proteomic Search Algorithm
    /// </summary>
    public class MorpheusSearchEngine : MSSearchEngine
    {
        public MorpheusSearchEngine()
        {
            DefaultPsmScoreType = PeptideSpectralMatchScoreType.Morpheus;
        }

        public override PeptideSpectralMatch Search(IMassSpectrum massSpectrum, Peptide peptide, FragmentTypes fragmentTypes, Tolerance productMassTolerance)
        {
            double[] eMasses = massSpectrum.MassSpectrum.GetMasses();
            double[] eIntenisties = massSpectrum.MassSpectrum.GetIntensities();
            double tic = massSpectrum.MassSpectrum.GetTotalIonCurrent();

            PeptideSpectralMatch psm = new PeptideSpectralMatch(DefaultPsmScoreType) {Peptide = peptide};
            double[] tMasses = peptide.Fragment(fragmentTypes).Select(frag => Mass.MzFromMass(frag.MonoisotopicMass, 1)).OrderBy(val => val).ToArray();
            double score = Search(eMasses, eIntenisties, tMasses, productMassTolerance, tic);
            psm.Score = score;

            return psm;
        }

        public override SortedMaxSizedContainer<PeptideSpectralMatch> Search(IMassSpectrum spectrum, IEnumerable<Peptide> peptides, FragmentTypes fragmentTypes, Tolerance productMassTolerance)
        {
            SortedMaxSizedContainer<PeptideSpectralMatch> results = new SortedMaxSizedContainer<PeptideSpectralMatch>(MaxMatchesPerSpectrum);

            double[] eMasses = spectrum.MassSpectrum.GetMasses();
            double[] eIntenisties = spectrum.MassSpectrum.GetIntensities();
            double tic = spectrum.MassSpectrum.GetTotalIonCurrent();
            ;

            foreach (var peptide in peptides)
            {
                PeptideSpectralMatch psm = new PeptideSpectralMatch(DefaultPsmScoreType) {Peptide = peptide};
                double[] tMasses =
                    peptide.Fragment(fragmentTypes)
                        .Select(frag => Mass.MzFromMass(frag.MonoisotopicMass, 1))
                        .OrderBy(val => val)
                        .ToArray();
                double score = Search(eMasses, eIntenisties, tMasses, productMassTolerance, tic);
                psm.Score = score;
                results.Add(psm);
            }

            return results;
        }

        //private double Search(ref double[] eMasses, ref double[] eIntenisties, double[] tMasses, double productTolerance, double tic, ref Dictionary<double, double> scores)
        //{
        //    double score = 0.0;

        //    int eLength = eMasses.Length;
        //    int tLength = tMasses.Length;
        //    int e = 0;

        //    foreach (double t in tMasses)
        //    {
        //        double storedScore;
        //        if (scores.TryGetValue(t, out storedScore))
        //        {
        //            score += storedScore;
        //            continue;
        //        }

        //        double minMZ = t - productTolerance;
        //        double maxMZ = t + productTolerance;

        //        while (e < eLength && eMasses[e] < minMZ)
        //            e++;

        //        if (e >= eLength)
        //            break;

        //        if (eMasses[e] > maxMZ)
        //            continue;

        //        double intensities = 0;
        //        int index = e; // switch variables to keep e the same for the next loop around
        //        do
        //        {
        //            intensities += eIntenisties[index];
        //            index++;
        //        } while (index < eLength && eMasses[index] < maxMZ);

        //        storedScore = 1 + intensities/tic;

        //        score += storedScore;
        //        scores[t] = storedScore;
        //    }

        //    return score;
        //}

        /// <summary>
        /// The main searching algorithm of Morpheus
        /// </summary>
        /// <param name="eMasses">The experimental masses</param>
        /// <param name="eIntenisties">The experimental intensities</param>
        /// <param name="tMasses">The theoretical masses</param>
        /// <param name="productTolerance">The product mass tolerance</param>
        /// <param name="tic">The total ion current of the experimental peaks</param>
        /// <returns></returns>
        private double Search(double[] eMasses, double[] eIntenisties, double[] tMasses, Tolerance productTolerance, double tic)
        {
            double score = 0.0;
            double intensities = 0.0;
            int eLength = eMasses.Length;
            int tLength = tMasses.Length;
            int e = 0;

            bool forceCheck = productTolerance.GetMinimumValue(tMasses[tLength - 1]) >= eMasses[eLength - 1];
            if (forceCheck)
            {
                foreach (double t in tMasses)
                {
                    IRange<double> range = productTolerance.GetRange(t);
                    double minMZ = range.Minimum;
                    double maxMZ = range.Maximum;

                    while (e < eLength && eMasses[e] < minMZ)
                        e++;

                    if (e >= eLength)
                        break;

                    if (eMasses[e] > maxMZ)
                        continue;

                    score++;

                    int index = e; // switch variables to keep e the same for the next loop around
                    do
                    {
                        intensities += eIntenisties[index];
                        index++;
                    } while (index < eLength && eMasses[index] < maxMZ);
                }
            }
            else
            {
                foreach (double t in tMasses)
                {
                    IRange<double> range = productTolerance.GetRange(t);
                    double minMZ = range.Minimum;
                    double maxMZ = range.Maximum;

                    while (eMasses[e] < minMZ)
                        e++;

                    if (eMasses[e] > maxMZ)
                        continue;

                    score++;

                    int index = e; // switch variables to keep e the same for the next loop around
                    do
                    {
                        intensities += eIntenisties[index];
                        index++;
                    } while (index < eLength && eMasses[index] < maxMZ);
                }
            }
            return score + intensities/tic;
        }
    }
}