// Copyright 2012, 2013, 2014 Derek J. Bailey
// 
// This file (MZPeak.cs) is part of CSMSL.
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