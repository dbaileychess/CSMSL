// Copyright 2022 Derek J. Bailey
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

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
