///////////////////////////////////////////////////////////////////////////
//  Peak.cs - A particular feature on a m/z spectrum                      /
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

namespace CSMSL.Spectral
{
    public class Peak : IPeak
    {
        public float Intensity { get; set; }
        public double MZ { get; set; }

        public Peak()
            : this(0, 0) { }

        public Peak(double mz, float intensity)
        {
            MZ = mz;
            Intensity = intensity;
        }

        public int CompareTo(IPeak other)
        {
            return MZ.CompareTo(other.MZ);
        }

        public bool Equals(IPeak other)
        {
            if (object.ReferenceEquals(this, other)) return true;
            return MZ.Equals(other.MZ) && Intensity.Equals(other.Intensity);
        }

        public override string ToString()
        {
            return string.Format("({0:G5}, {1:G5})", MZ, Intensity);
        }
    }
}