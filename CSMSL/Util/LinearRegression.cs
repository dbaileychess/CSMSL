using System;

namespace CSMSL.Util
{
    public class LinearRegression
    {
        private LinearRegression(double slope, double intercept)
        {
            Slope = slope;
            Intercept = intercept;
        }

        public double Intercept { get; private set; }

        public double RSquared { get; private set; }

        public double Slope { get; private set; }

        public static LinearRegression Calculate(double[,] xy)
        {
            int length = xy.GetLength(0);
            double y_avg, sxy, sxx, syy, sserr;
            double x_avg = y_avg = sxy = sxx = syy = sserr = 0;

            for (int i = 0; i < length; i++)
            {
                x_avg += xy[i, 0];
                y_avg += xy[i, 1];
            }
            x_avg /= length;
            y_avg /= length;

            for (int i = 0; i < length; i++)
            {
                double xdiff = xy[i, 0] - x_avg;
                double ydiff = xy[i, 1] - y_avg;
                sxy += xdiff * ydiff;
                sxx += xdiff * xdiff;
                syy += ydiff * ydiff;
            }
            double m = sxy / sxx;
            double b = y_avg - m * x_avg;
            LinearRegression regression = new LinearRegression(m, b);
            for (int i = 0; i < length; i++)
            {
                sserr += Math.Pow(xy[i, 1] - regression.GetY(xy[i, 0]), 2);
            }
            regression.RSquared = 1 - (sserr / syy);
            return regression;
        }

        public static LinearRegression Calculate(double[] x, double[] y)
        {
            int length = x.Length;
            if (length != y.Length)
            {
                throw new ArgumentException("X and Y Dimensions do not match");
            }
            double y_avg, sxy, sxx, syy, sserr;
            double x_avg = y_avg = sxy = sxx = syy = sserr = 0;

            for (int i = 0; i < length; i++)
            {
                x_avg += x[i];
                y_avg += y[i];
            }
            x_avg /= length;
            y_avg /= length;

            for (int i = 0; i < length; i++)
            {
                double xdiff = x[i] - x_avg;
                double ydiff = y[i] - y_avg;
                sxy += xdiff * ydiff;
                sxx += xdiff * xdiff;
                syy += ydiff * ydiff;
            }
            double m = sxy / sxx;
            double b = y_avg - m * x_avg;
            LinearRegression regression = new LinearRegression(m, b);
            for (int i = 0; i < length; i++)
            {
                sserr += Math.Pow(y[i] - regression.GetY(x[i]), 2);
            }
            regression.RSquared = 1 - (sserr / syy);
            return regression;
        }

        public double GetX(double y)
        {
            return (y - Intercept) / Slope;
        }

        public double GetY(double x)
        {
            return Slope * x + Intercept;
        }

        public override string ToString()
        {
            return string.Format("y = {0:F2}x + {1:F2} (R^2 = {2:F4})", Slope, Intercept, RSquared);
        }
    }
}