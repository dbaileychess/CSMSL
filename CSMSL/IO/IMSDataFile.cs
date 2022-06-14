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
using CSMSL.Proteomics;
using CSMSL.Spectral;
using System.Collections.Generic;

namespace CSMSL.IO
{
    public interface IMSDataFile : IEnumerable<IMSDataScan>, IDisposable, IEquatable<IMSDataFile>
    {
        void Open();
        string Name { get; }
        bool IsOpen { get; }
        int FirstSpectrumNumber { get; }
        int LastSpectrumNumber { get; }
        int GetMsnOrder(int spectrumNumber);
        double GetInjectionTime(int spectrumNumber);
        double GetPrecursorMz(int spectrumNumber, int msnOrder = 2);
        double GetRetentionTime(int spectrumNumber);
        DissociationType GetDissociationType(int spectrumNumber, int msnOrder = 2);
        Polarity GetPolarity(int spectrumNumber);
        ISpectrum GetSpectrum(int spectrumNumber);
        IMSDataScan this[int spectrumNumber] { get; }
    }

    public interface IMSDataFile<out TSpectrum> : IMSDataFile, IEnumerable<IMSDataScan<TSpectrum>>
        where TSpectrum : ISpectrum
    {
        new TSpectrum GetSpectrum(int spectrumNumber);
        new IMSDataScan<TSpectrum> this[int spectrumNumber] { get; }
    }
}