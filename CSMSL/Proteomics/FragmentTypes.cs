///////////////////////////////////////////////////////////////////////////
//  FragmentType.cs - A collection of common fragment types               /
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

namespace CSMSL.Proteomics
{
    [Flags]
    public enum FragmentTypes
    {
        None = 0,
        a = 1,
        adot = 2,
        b = 4,
        bdot = 8,
        c = 16,
        cdot = 32,
        x = 64,
        xdot = 128,
        y = 256,
        ydot = 512,
        z = 1024,
        zdot = 2048,
        Internal = 4096,    
        All = Int32.MaxValue
    }
}