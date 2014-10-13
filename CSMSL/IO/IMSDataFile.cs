// Copyright 2012, 2013, 2014 Derek J. Bailey
// 
// This file (IMSDataFile.cs) is part of CSMSL.
// 
// CSMSL is free software: you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// CSMSL is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public
// License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with CSMSL. If not, see <http://www.gnu.org/licenses/>.

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