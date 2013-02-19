using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSMSL.Analysis.ExperimentalDesign
{
    public abstract class Replicate
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Number { get; set; }
    }
}
