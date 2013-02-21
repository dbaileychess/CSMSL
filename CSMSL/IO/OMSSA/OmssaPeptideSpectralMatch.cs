using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMSL.Analysis.Identification;
using CsvHelper.Configuration;

namespace CSMSL.IO.OMSSA
{
    public class OmssaPeptideSpectralMatch
    {
        [CsvField(Name = "Spectrum number")]
        public int SpectrumNumber { get; set; }


    }
}
