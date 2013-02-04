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
    public sealed class Isotope
    {        
        internal Isotope(Element parentElement, int massNumber, double mass, float abundance)
        {
            Element = parentElement;
            MassNumber = massNumber;
            Mass = mass;
            RelativeAbundance = abundance;
        }

        public string AtomicSymbol
        {
            get { return Element.AtomicSymbol; }
        }

        internal int UniqueID { get; set; }
        internal bool IsPrincipalIsotope { get; set; }

        public Element Element { get; private set; }            
       
        public double Mass { get; private set; }

        public int MassNumber { get; private set; }

        public float RelativeAbundance { get; private set; }     

        public override string ToString()
        {
            return string.Format("{0}{1:G0}", AtomicSymbol, MassNumber);
        }
    }
}