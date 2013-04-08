using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;
using CSMSL.Analysis.Quantitation;

namespace CSMSL.Analysis.ExperimentalDesign
{
    public class ExperimentalSet
    {       
        private Dictionary<IQuantitationChannel, ExperimentalCondition> _data;

        public string Name { get; private set; }

        public ExperimentalSet(string name = "")
        {
            Name = name;
            _data = new Dictionary<IQuantitationChannel, ExperimentalCondition>();
        }

        public void Add(ExperimentalCondition condition, IQuantitationChannel quantChannel)
        {
            if (_data.ContainsKey(quantChannel))
            {
                throw new DuplicateKeyException("Cannot add two identical quantitation channels to an experimental set!");
            }
            else
            {
                _data.Add(quantChannel, condition);
            }
        }        

    }
}
