using CSMSL.Chemistry;
using System;

namespace CSMSL.IO.OMSSA
{
    public class OmssaModification : IMass
    {
        public string Name { get; set; }

        public OmssaModification(string name, double mono, double average)
        {
            Name = name;
            _mass = new Mass(mono, average);
        }

        private Mass _mass;

        public Mass Mass
        {
            get { return _mass; }
        }

        public override string ToString()
        {
            return Name;
        }

        public double MonoisotopicMass
        {
            get { throw new NotImplementedException(); }
        }
    }
}
