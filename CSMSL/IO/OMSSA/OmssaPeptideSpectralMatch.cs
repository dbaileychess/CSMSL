using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace CSMSL.IO.OMSSA
{

    public class OmssaPeptideSpectralMatch : CsvClassMap<OmssaPeptideSpectralMatch>
    {
        public override void CreateMap()
        {
            Map(m => m.SpectrumNumber).Name("Spectrum number");
            Map(m => m.EValue).Name("E-value");
            Map(m => m.Mass).Name("Mass");
            Map(m => m.TheoreticalMass).Name("Theo Mass");
            Map(m => m.Sequence).Name("Peptide");
            Map(m => m.Defline).Name("Defline");
            Map(m => m.FileName).Name("Filename/id");
            Map(m => m.Accession).Name("Accession");
            Map(m => m.PValue).Name("P-value");
            Map(m => m.Modifications).Name("Mods");
            Map(m => m.Charge).Name("Charge");
            Map(m => m.StartResidue).Name("Start");
            Map(m => m.StopResidue).Name("Stop");
            Map(m => m.GI).Name("gi");
            Map(m => m.NistScore).Name("NIST score");
        }

        public int SpectrumNumber { get; set; }


        public double EValue { get; set; }


        public double Mass { get; set; }

        public double TheoreticalMass { get; set; }


        public string Sequence { get; set; }


        public string Defline { get; set; }


        public string FileName { get; set; }


        public string Accession { get; set; }


        public double PValue { get; set; }


        public string Modifications { get; set; }


        public int Charge { get; set; }


        public int StartResidue { get; set; }


        public int StopResidue { get; set; }


        public int GI { get; set; }

        public double NistScore { get; set; }
    }
}
