using System;

namespace CSMSL.Proteomics
{
    [Flags]
    public enum FragmentType
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