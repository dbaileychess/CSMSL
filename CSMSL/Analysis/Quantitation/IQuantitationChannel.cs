using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMSL.Chemistry;

namespace CSMSL.Analysis.Quantitation
{
    public interface IQuantitationChannel: IMass
    {   
        /// <summary>
        /// Does this channel depend upon the peptide sequence to calculate its mass?
        /// </summary>
        bool IsSequenceDependent { get; }

        /// <summary>
        /// The mass of the reporter
        /// </summary>
        Mass ReporterMass { get; }
    }
}
