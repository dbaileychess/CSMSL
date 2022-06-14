﻿// Copyright 2022 Derek J. Bailey
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

using CSMSL.Spectral;

namespace CSMSL.IO
{
    public interface IMSDataScan : IMassSpectrum
    {
        int SpectrumNumber { get; }
        int MsnOrder { get; }
        double RetentionTime { get; }
        Polarity Polarity { get; }
        MZAnalyzerType MzAnalyzer { get; }
        DoubleRange MzRange { get; }
    }

    public interface IMSDataScan<out TSpectrum> : IMSDataScan
        where TSpectrum : ISpectrum
    {
        new TSpectrum MassSpectrum { get; }
    }
}