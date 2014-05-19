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

        public ThermoSpectrum(double[,] peakData)
            : base(peakData)
        {
            _noises = new double[Count];
            var charges = new double[Count];

            Buffer.BlockCopy(peakData, 8 * Count * 2, _noises, 0, 8 * Count);
            Buffer.BlockCopy(peakData, 8 * Count * 3, charges, 0, 8 * Count);
            _charges = new int[Count];
            for (int i = 0; i < Count; i++)
            {
                _charges[i] = (int)charges[i];
            }
        }

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

        public new ThermoLabeledPeak GetBasePeak()
        {
            int index = _intensities.MaxIndex();
            return GetPeak(index);
        }

        public new ThermoLabeledPeak GetPeak(int index)
        {
            return new ThermoLabeledPeak(_masses[index], _intensities[index], _charges[index], _noises[index]);
        }

        public new ThermoLabeledPeak GetClosestPeak(IRange<double> massRange)
        {
            double mean = (massRange.Maximum + massRange.Minimum) / 2.0;
            double width = massRange.Maximum - massRange.Minimum;
            return GetClosestPeak(mean, width);
        }

        public new ThermoLabeledPeak GetClosestPeak(double mean, double tolerance)
        {
            int index = GetClosestPeakIndex(mean, tolerance);

            return index >= 0 ? GetPeak(index) : null;
        }
    }
}
