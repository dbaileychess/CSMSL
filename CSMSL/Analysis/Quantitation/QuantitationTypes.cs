using System;

namespace CSMSL.Analysis.Quantitation
{
    [Flags]
    public enum QuantitationTypes
    {
        None = 0,
        ReporterTag = 1,
        SILAC = 2,
        Chemical = 4,
        Metabolic = 8,
        MS1Based = 16,
        MS2Based = 32
    }
}
