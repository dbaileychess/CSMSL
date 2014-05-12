using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSMSL.Spectral;

namespace CSMSL.IO.Thermo
{
    public class ThermoSpectrum : Spectrum
    {
        private readonly double[] _noises;
        private readonly int[] _charges;

        public ThermoSpectrum(double[] mz, double[] intensity, double[] noise, int[] charge, bool shouldCopy = true)
            : base(mz, intensity, shouldCopy)
        {
            if (shouldCopy)
            {
                _noises = new double[Count];
                _charges = new int[Count];

                Buffer.BlockCopy(noise, 0, _noises, 0, sizeof(double) * Count);
                Buffer.BlockCopy(charge, 0, _charges, 0, sizeof(int) * Count);
            }
            else
            {
                _noises = noise;
                _charges = charge;
            }
        }
        
        public double GetNoise(int index)
        {
            return _noises[index];
        }

        public double GetCharge(int index)
        {
            return _charges[index];
        }

        public double[] GetNoises()
        {
            return (double[])_noises.Clone();
        }

        public int[] GetCharges()
        {
            return (int[])_charges.Clone();
        }
    }
}
