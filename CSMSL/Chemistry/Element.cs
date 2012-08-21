///////////////////////////////////////////////////////////////////////////
//  Element.cs - A collection of naturally occuring isotopes              /
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

using System.Collections.Generic;

namespace CSMSL.Chemistry
{
    public class Element : IEnumerable<Isotope>
    {
        private int _atomicNumber;
        private string _atomicSymbol;

        private double _avgmass = 0;

        private Dictionary<int, Isotope> _isotopes;

        private double _mass = 0;

        internal Isotope _principal = null;

        private string _name;

        public Isotope this[int atomicNumber]
        {
            get
            {
                return _isotopes[atomicNumber];
            }
        }

        public Element(string name, string symbol, int atomicNumber)
        {
            _name = name;
            _atomicSymbol = symbol;
            _atomicNumber = atomicNumber;
            _isotopes = new Dictionary<int, Isotope>();
        }

        public int AtomicNumber
        {
            get { return _atomicNumber; }
            set { _atomicNumber = value; }
        }

        public string AtomicSymbol
        {
            get { return _atomicSymbol; }
            set { _atomicSymbol = value; }
        }

        public double AverageMass
        {
            get
            {
                return _avgmass;
            }
        }

        public double Mass
        {
            get
            {
                return _mass;
            }
        }

        public Isotope Principal
        {
            get
            {
                return _principal;
            }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public IEnumerator<Isotope> GetEnumerator()
        {
            return _isotopes.Values.GetEnumerator();
        }

        public override string ToString()
        {
            return _atomicSymbol;
        }

        public override int GetHashCode()
        {
            return _atomicNumber;
        }

        private double _totalAbundance = 0;
        private double _totalMass = 0;

        public Isotope AddIsotope(int atomicNumber, double mass, float abundance)
        {
            Isotope isotope = new Isotope(this, atomicNumber, mass, abundance);
            if (!_isotopes.ContainsKey(atomicNumber))
            {
                _isotopes.Add(atomicNumber, isotope);
                _totalAbundance += abundance;
                _totalMass += abundance * mass;
                _avgmass = _totalMass / _totalAbundance;
                if (_principal == null || abundance > _principal.RelativeAbundance)
                {
                    if (_principal != null)
                    {
                        _principal._isPrincipalIsotope = false;
                    }
                    _principal = isotope;
                    _principal._isPrincipalIsotope = true;
                    _mass = mass;
                }
            }
            return isotope;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _isotopes.Values.GetEnumerator();
        }
    }
}