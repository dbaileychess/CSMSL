///////////////////////////////////////////////////////////////////////////
//  Spectrum.cs - A collection of peaks                                   /
//                                                                        /
//  Copyright 2012 Derek J. Bailey                                        /
//  This file is part of CSMSL.                                           /
//                                                                        /
//  CSMSL is free software: you can redistribute it and/or modify         /
//  it under the terms of the GNU General Public License as published by  /
//  the Free Software Foundation, either version 3 of the License, or     /
//  (at your option) any later version.                                   /
//                                                                        /
//  CSMSL is distributed in the hope that it will be useful,              /
//  but WITHOUT ANY WARRANTY; without even the implied warranty of        /
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the         /
//  GNU General Public License for more details.                          /
//                                                                        /
//  You should have received a copy of the GNU General Public License     /
//  along with CSMSL.  If not, see <http://www.gnu.org/licenses/>.        /
///////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;

namespace CSMSL.Spectral
{
    public class MassSpectrum : Spectrum<MZPeak>, IMassSpectrum, IEnumerable<MZPeak>
    {
        public MZPeak BasePeak { get; protected set; }

        public double TotalIonCurrent { get; protected set; }

        public MassSpectrum Filter(double miniumSN)
        {
            double noiseLevel = GetNoiseLevel();
            double minIntensity = miniumSN * noiseLevel;
            return new MassSpectrum(this.Where(p => p.Intensity > minIntensity));
        }

        public double GetNoiseLevel()
        {
            return Peaks.Average(p => p.Intensity);
        }

        public MassSpectrum() { }        

        public MassSpectrum(double[,] data)          
        {
            LoadData(data);
        }

        public MassSpectrum(double[] mzs, double[] intensities)          
        {
            LoadData(mzs, intensities);
        }        

        public MassSpectrum(IEnumerable<MZPeak> peaks)
        {
            LoadData(peaks);
        }

        protected void LoadData(IEnumerable<MZPeak> peaks)
        {
            Peaks = peaks.ToArray();
            Count = Peaks.Length;
            TotalIonCurrent = 0;
            double maxInt = 0;
            for (int i = 0; i < Count; i++)
            {
                MZPeak peak = Peaks[i];
                TotalIonCurrent += peak.Intensity;
                if (peak.Intensity > maxInt)
                {
                    maxInt = peak.Intensity;
                    BasePeak = peak;
                }
            }
        }

        private void LoadData(double[] mzs, double[] intensities)
        {
            int length;
            if ((length = mzs.Length) != intensities.Length)
            {
                throw new FormatException("M/Z and Intensities arrays are not the same dimensions");
            }
            Count = length;
            TotalIonCurrent = 0;
            Peaks = new MZPeak[Count];
            double maxInt = 0;
            for (int i = 0; i < Count; i++)
            {
                double intensity = intensities[i];
                Peaks[i] = new MZPeak(mzs[i], intensity);
                TotalIonCurrent += intensity;
                if (intensity > maxInt)
                {
                    maxInt = intensity;
                    BasePeak = Peaks[i];
                }
            }
        }

        private void LoadData(double[,] data)
        {
            Count = data.GetLength(0);
            TotalIonCurrent = 0;
            Peaks = new MZPeak[Count];
            double maxInt = 0;
            for (int i = 0; i < Count; i++)
            {
                double intensity = data[i, 1];
                Peaks[i] = new MZPeak(data[i, 0], intensity);
                TotalIonCurrent += intensity;
                if (intensity > maxInt)
                {
                    maxInt = intensity;
                    BasePeak = Peaks[i];
                }
            }
        }

        public override Spectrum<MZPeak> Clone()
        {
            return new MassSpectrum(Peaks);
        }

        MassSpectrum IMassSpectrum.MassSpectrum
        {
            get { return this; }
        }

        public double[] GetMasses()
        {
            return Peaks.Select(p => p.MZ).ToArray();
        }

        public double[] GetIntensities()
        {
            return Peaks.Select(p => p.Intensity).ToArray();
        }

        public IEnumerator<MZPeak> GetEnumerator()
        {
            return ((MZPeak[])Peaks).ToList().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Peaks.GetEnumerator();
        }

        public MZPeak GetClosestPeak(MassRange mzRange)
        {
            List<MZPeak> peaks = new List<MZPeak>();
            if (TryGetPeaks(mzRange, out peaks))
            {
                double mz = mzRange.Mean;
                if (peaks.Count == 1)
                {
                    return peaks[0];
                }
                else if (peaks.Count == 2)
                {
                    if (Math.Abs(peaks[0].MZ - mz) < Math.Abs(peaks[1].MZ - mz))
                    {
                        return peaks[0];
                    }
                    else
                    {
                        return peaks[1];
                    }
                }
                else
                {

                    double smallestDiff = double.MaxValue;
                    MZPeak bestPeak = null;

                    foreach (MZPeak peak in peaks)
                    {
                        double diff = Math.Abs(peak.MZ - mz);
                        if (diff < smallestDiff)
                        {
                            smallestDiff = diff;
                            bestPeak = peak;
                        }
                    }
                    return bestPeak;
                }
            }
            else
            {
                return null;
            }
        }
    }   
    
}