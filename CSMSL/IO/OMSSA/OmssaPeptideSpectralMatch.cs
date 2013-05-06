using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMSL.Proteomics;
using CSMSL.Analysis.Identification;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace CSMSL.IO.OMSSA
{
    public class OmssaPeptideSpectralMatch
    {
        [CsvField(Name = "Spectrum number")]
        public int SpectrumNumber { get; set; }

        [CsvField(Name = "E-value")]
        public double EValue { get; set; }

        [CsvField(Name = "Mass")]
        public double Mass { get; set; }
        
        [CsvField(Name = "Theo Mass")]
        public double TheoreticalMass { get; set; }
             
        [CsvField(Name = "Peptide")]     
        public string Sequence { get; set; }
 
        [CsvField(Name = "Defline")]
        public string Defline { get; set; }

        [CsvField(Name = "Filename/id")]
        public string FileName { get; set; }

        [CsvField(Name = "Accession")]
        public string Accession { get; set; }

        [CsvField(Name = "P-value")]
        public double PValue { get; set; }

        [CsvField(Name = "Mods")]
        public string Modifications { get; set; }
         
        [CsvField(Name = "Charge")]
        public int Charge { get; set; }

        [CsvField(Name = "Start")]
        public int StartResidue { get; set; }

        [CsvField(Name = "Stop")]
        public int StopResidue { get; set; }       

        [CsvField(Name = "gi")]
        public int GI { get; set; }
        
        [CsvField(Name = "NIST score")]
        public double NistScore { get; set; }
    }
}
