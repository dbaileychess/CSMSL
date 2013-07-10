using System;


namespace CSMSL.Spectral
{
    public class ChromatographicPeak : IPeak
    {
        public double Time { get; protected set; }

        public double Intensity { get; protected set; }

        public ChromatographicPeak(double time, double intensity)
        {
            Time = time;
            Intensity = intensity;
        }

        public void CombinePoints(ChromatographicPeak otherPoint)
        {
            if (!Time.Equals(otherPoint.Time))
            {
                throw new ArgumentException("The two chromatogram points don't have the same time");
            }
            Intensity += otherPoint.Intensity;           
        }

        public override string ToString()
        {
            return string.Format("({0:G4}, {1:G4})", Time, Intensity);
        }

        public int CompareTo(double time)
        {
            return Time.CompareTo(time);
        }

        public int CompareTo(IPeak other)
        {
            return Time.CompareTo(other.X);
        }

        public int CompareTo(ChromatographicPeak other)
        {
            return Time.CompareTo(other.Time);
        }

        public int CompareTo(object other)
        {
            return 0;
        }

        double IPeak.X
        {
            get { return Time; }
        }

        double IPeak.Y
        {
            get { return Intensity; }
        }
    }
}
