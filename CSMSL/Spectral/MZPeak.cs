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

using System;

namespace CSMSL.Spectral
{
    /// <summary>
    /// A peak in a mass spectrum that has a well defined m/z and intenisty value
    /// </summary>
    public class MZPeak : IPeak, IEquatable<MZPeak>
    {
        private readonly double _intensity;
        private readonly double _mz;

        public double Intensity { get { return _intensity; } }
        public double MZ { get { return _mz; } }
        
        public MZPeak(double mz, double intensity)
        {
            _mz = mz;
            _intensity = intensity;
        }       

        public bool Equals(IPeak other)
        {
            if (ReferenceEquals(this, other)) return true;
            return MZ.Equals(other.X) && Intensity.Equals(other.Y);
        }

        public override string ToString()
        {
            return string.Format("({0:G5},{1:G5})", MZ, Intensity);
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
            throw new InvalidOperationException("Unable to compare types");
        }

        double IPeak.X
        {
            get { return _mz; }
        }

        double IPeak.Y
        {
            get { return _intensity; }
        }

        public override bool Equals(object obj)
        {
            return obj is MZPeak && Equals((MZPeak)obj);
        }

        public override int GetHashCode()
        {
            return _mz.GetHashCode() ^ _intensity.GetHashCode();
        }

        public bool Equals(MZPeak other)
        {
            return MZ == other.MZ && Intensity == other.Intensity;
        }
    }
}