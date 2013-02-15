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
    public class MZPeak : IPeak
    {
        public double Intensity { get; private set; }
        public double MZ { get; private set; }

        public MZPeak()
            : this(0, 0) { }

        public MZPeak(double mz, double intensity)
        {
            MZ = mz;
            Intensity = intensity;
        }       

        public bool Equals(IPeak other)
        {
            if (object.ReferenceEquals(this, other)) return true;
            return MZ.Equals(other.X) && Intensity.Equals(other.Y);
        }

        public override string ToString()
        {
            return string.Format("({0:G5}, {1:G5})", MZ, Intensity);
        }        
                
        public int CompareTo(double other)
        {
            return MZ.CompareTo(other);
        }

        public int CompareTo(IPeak other)
        {
            return MZ.CompareTo(other.X);
        }

        public int CompareTo(object other)
        {
            if (other is double)
                return MZ.CompareTo((double)other);
            if (other is IPeak)
                return CompareTo((IPeak)other);
            throw new System.InvalidOperationException("Unable to compare types");
        }

        double IPeak.X
        {
            get { return MZ; }
        }

        double IPeak.Y
        {
            get { return Intensity; }
        }

    }
}