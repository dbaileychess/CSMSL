///////////////////////////////////////////////////////////////////////////
//  Isotope.cs - A single isotope of an element                           /
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

namespace CSMSL.Chemistry
{
    public class Isotope : IEquatable<Isotope>
    {
        internal bool _isPrincipalIsotope;
        internal int _uniqueID;
        private float _abundance;
        private Element _element;

        private int _hashCode;
        private double _mass;
        private int _massNumber;

        internal Isotope(Element parentElement, int massNumber, double mass, float abundance)
        {
            _element = parentElement;
            _massNumber = massNumber;
            _mass = mass;
            _abundance = abundance;
            _hashCode = 13 + (_massNumber << 5) + (_element.AtomicNumber >> 2 + 12);
        }

        public string AtomicSymbol
        {
            get { return _element.AtomicSymbol; }
        }

        public Element Element
        {
            get { return _element; }
        }

        public bool IsPrincipalIsotope
        {
            get
            {
                return _isPrincipalIsotope;
            }
            set
            {
                _isPrincipalIsotope = value;
            }
        }

        public double Mass
        {
            get { return _mass; }
            private set { _mass = value; }
        }

        public int MassNumber
        {
            get { return _massNumber; }
            private set { _massNumber = value; }
        }

        public float RelativeAbundance
        {
            get { return _abundance; }
            private set { _abundance = value; }
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public bool Equals(Isotope other)
        {
            return _massNumber.Equals(other._massNumber);
        }

        public override string ToString()
        {
            return string.Format("{0}{1:G0}", AtomicSymbol, _massNumber);
        }
    }
}