using System;
using CSMSL.Spectral;

namespace CSMSL.IO.Thermo
{
    public class ThermoSpectrum : Spectrum<ThermoLabeledPeak>
    {
        private readonly double[] _noises;
        private readonly double[] _resolutions;
        private readonly int[] _charges;
        
        internal ThermoSpectrum(double[,] peakData, int count)
            : base(peakData, count)
        {
            int arrayLength = peakData.GetLength(1);
            _noises = new double[Count];
            _resolutions = new double[Count];
            var charges = new double[Count];

            Buffer.BlockCopy(peakData, sizeof(double) * arrayLength * (int)ThermoRawFile.RawLabelDataColumn.Resolution, _resolutions, 0, sizeof(double) * Count);
            Buffer.BlockCopy(peakData, sizeof(double) * arrayLength * (int)ThermoRawFile.RawLabelDataColumn.NoiseLevel, _noises, 0, sizeof(double) * Count);
            Buffer.BlockCopy(peakData, sizeof(double) * arrayLength * (int)ThermoRawFile.RawLabelDataColumn.Charge, charges, 0, sizeof(double) * Count);

            _charges = new int[Count];
            for (int i = 0; i < Count; i++)
            {
                _charges[i] = (int)charges[i];
            }
        }

        internal ThermoSpectrum(double[,] peakData)
            : this(peakData, peakData.GetLength(1)) { }
 
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

        public double GetSignalToNoise(int index)
        {
            double noise = _noises[index];
            if (noise == 0)
                return 0;
            return _intensities[index] / noise;
        }
        
        public int GetCharge(int index)
        {
            return _charges[index];
        }

        public double GetResolution(int index)
        {
            return _resolutions[index];
        }

        public double[] GetNoises()
        {
            double[] noises = new double[Count];
            Buffer.BlockCopy(_noises, 0, noises, 0, sizeof(double) * Count);
            return noises;
        }

        public double[] GetResolutions()
        {
            double[] resolutions = new double[Count];
            Buffer.BlockCopy(_resolutions, 0, resolutions, 0, sizeof(double) * Count);
            return resolutions;
        }

        public int[] GetCharges()
        {
            int[] charges = new int[Count];
            Buffer.BlockCopy(_charges, 0, charges, 0, sizeof(int) * Count);
            return charges;
        }
        
        public override ThermoLabeledPeak GetPeak(int index)
        {
            return new ThermoLabeledPeak(_masses[index], _intensities[index], _charges[index], _noises[index], _resolutions[index]);
        }
        
        public override byte[] ToBytes(bool zlibCompressed = false)
        {
            throw new NotImplementedException();
        }

 
    }
}
