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

using System;
using System.Security.Cryptography.X509Certificates;

namespace CSMSL.Spectral
{
    /// <summary>
    /// A peak in a mass spectrum that has a well defined m/z and intenisty value
    /// </summary>
    public class MZPeak : IPeak, IEquatable<MZPeak>
    {
        public double Intensity { get; private set; }

        public double MZ { get; private set; }

        public MZPeak(double mz = 0.0, double intensity = 0.0)
        {
            MZ = mz;
            Intensity = intensity;
        }

        public bool Equals(IPeak other)
        {
            if (ReferenceEquals(this, other)) return true;
            return MZ.FussyEquals(other.X) && Intensity.FussyEquals(other.Y);
        }

        public override string ToString()
        {
            return string.Format("({0:F4},{1:G5})", MZ, Intensity);
        }

        public int CompareTo(double other)
        {
            return MZ.CompareTo(other);
        }

        public int CompareTo(IPeak other)
        {
            if (other == null)
                return 1;
            return MZ.CompareTo(other.X);
        }

        public int CompareTo(object other)
        {
            if (other is double)
                return MZ.CompareTo((double) other);
            var peak = other as IPeak;
            if (peak != null)
                return CompareTo(peak);
            throw new InvalidOperationException("Unable to compare types");
        }

        protected double X
        {
            get { return MZ; }
        }

        protected double Y
        {
            get { return Intensity; }
        }

        double IPeak.X
        {
            get { return X; }
        }

        double IPeak.Y
        {
            get { return Y; }
        }

        public override bool Equals(object obj)
        {
            return obj is MZPeak && Equals((MZPeak) obj);
        }

        public override int GetHashCode()
        {
            return MZ.GetHashCode() ^ Intensity.GetHashCode();
        }

        public bool Equals(MZPeak other)
        {
            // Odd to use mass equals on intensity, might have to make that more generic sometime
            return MZ.FussyEquals(other.MZ) && Intensity.FussyEquals(other.Intensity);
        }
    }
}