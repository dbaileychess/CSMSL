///////////////////////////////////////////////////////////////////////////
//  IPeak.cs - An object that contains a m/z peak                         /
//                                                                        /
//  Copyright 2012 Derek J. Bailey                                        /
//  This file is part of CSMSL.                                           /
//                                                                        /
//  CSMSL is free software: you can redistribute it and/or modify         /
//  it under the terms of the GNU General Public License as published by  /
//  the Free Software Foundation, either version 3 of the License, or     /
//  (at your option) any later version.                                   /
//                                                                        /
//  CSMSL is distributed in the hope that it will be useful,              /
//  but WITHOUT ANY WARRANTY; without even the implied warranty of        /
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the         /
//  GNU General Public License for more details.                          /
//                                                                        /
//  You should have received a copy of the GNU General Public License     /
//  along with CSMSL.  If not, see <http://www.gnu.org/licenses/>.        /
///////////////////////////////////////////////////////////////////////////

using System;

namespace CSMSL.Spectral
{
    public interface IMZPeak : IPeak
    {
        /// <summary>
        /// The intensity of this peak
        /// <remarks>The float type is used here to reduce memory
        /// since high precision of intensity is not as imporant
        /// as it is with m/z</remarks>
        /// </summary>
        float Intensity { get; }

        /// <summary>
        /// The mass-to-charge of this peak
        /// </summary>
        double MZ { get; }
    }
}