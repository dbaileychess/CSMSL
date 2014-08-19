using System;
using System.Collections.Generic;
using CSMSL.IO;
using CSMSL.IO.Thermo;
using CSMSL.Spectral;

namespace CSMSL.Examples
{
    public static class ThermoRawFileExamples
    {
        public static void RecalibrateThermoRawFile()
        {
            List<ISpectrum> spectra = new List<ISpectrum>();
            using (ThermoRawFile rawFile = new ThermoRawFile("Resources/ThermoRawFileMS1MS2.raw"))
            {
                rawFile.Open();
                for (int i = rawFile.FirstSpectrumNumber; i <= rawFile.LastSpectrumNumber; i++)
                {
                    ThermoSpectrum spectrum = rawFile.GetLabeledSpectrum(i);
                    ThermoSpectrum correctedSpectrum = spectrum.CorrectMasses((mz) => mz - 5); // shift all masses 5 Th lower
                    spectra.Add(correctedSpectrum);
                }
            }
        }

        public static void LoopOverEveryScan()
        {
            using (ThermoRawFile rawFile = new ThermoRawFile("Resources/ThermoRawFileMS1MS2.raw"))
            {
                rawFile.Open();
                foreach (var scan in rawFile)
                {
                    Console.WriteLine("{0,-4} {1,3} {2,-6:F4} {3,-5} {4,7} {5,-10} {6} {7}", scan.SpectrumNumber, scan.MsnOrder, scan.RetentionTime,
                      scan.Polarity, scan.MassSpectrum.Count, scan.MzAnalyzer, scan.MzRange, scan.MassSpectrum.IsHighResolution);
                }
            }
        }
    }
}
