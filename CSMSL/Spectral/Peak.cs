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
        internal double _intensity;
        internal double _mz;

        public Peak()
            : this(0, 0) { }

        public Peak(double mz, double intensity)
        {
            _mz = mz;
            _intensity = intensity;
        }

        public double Intensity
        {
            get { return _intensity; }
            set { _intensity = value; }
        }

        public double MZ
        {
            get { return _mz; }
            set { _mz = value; }
        }

        public int CompareTo(IPeak other)
        {
            return _mz.CompareTo(other.MZ);
        }

        public bool Equals(IPeak other)
        {
            if (object.ReferenceEquals(this, other)) return true;
            return _mz.Equals(other.MZ) && _intensity.Equals(other.Intensity);
        }

        public override string ToString()
        {
            return string.Format("({0:G5}, {1:G5})", _mz, _intensity);
        }
    }
}