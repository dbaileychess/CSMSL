// Copyright 2012, 2013, 2014 Derek J. Bailey
// 
// This file (ChromatographicPeak.cs) is part of CSMSL.
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

namespace CSMSL.Spectral
{
    public sealed class ChromatographicPeak : IPeak
    {
        public double Time { get; private set; }

        public double Intensity { get; private set; }

        public ChromatographicPeak(double time, double intensity)
        {
            Time = time;
            Intensity = intensity;
        }

        public override string ToString()
        {
            return string.Format("({0:G4}, {1:G4})", Time, Intensity);
        }

        public int CompareTo(double time)
        {
            return Time.CompareTo(time);
        }

        public int CompareTo(IPeak other)
        {
            return Time.CompareTo(other.X);
        }

        public int CompareTo(ChromatographicPeak other)
        {
            return Time.CompareTo(other.Time);
        }

        public int CompareTo(object other)
        {
            return 0;
        }

        double IPeak.X
        {
            get { return Time; }
        }

        double IPeak.Y
        {
            get { return Intensity; }
        }
    }
}