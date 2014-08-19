// Copyright 2012, 2013, 2014 Derek J. Bailey
// 
// This file (LinearRegression.cs) is part of CSMSL.
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

        public double GetX(double y)
        {
            return (y - Intercept)/Slope;
        }

        public double GetY(double x)
        {
            return Slope*x + Intercept;
        }

        public override string ToString()
        {
            return string.Format("y = {0:F2}x + {1:F2} (R^2 = {2:F4})", Slope, Intercept, RSquared);
        }

        #region Static

        public static LinearRegression Calculate(double[,] xy)
        {
            int length = xy.GetLength(0);
            double yAvg, sxy, sxx, syy, sserr;
            double xAvg = yAvg = sxy = sxx = syy = sserr = 0;

            for (int i = 0; i < length; i++)
            {
                xAvg += xy[i, 0];
                yAvg += xy[i, 1];
            }
            xAvg /= length;
            yAvg /= length;

            for (int i = 0; i < length; i++)
            {
                double xdiff = xy[i, 0] - xAvg;
                double ydiff = xy[i, 1] - yAvg;
                sxy += xdiff*ydiff;
                sxx += xdiff*xdiff;
                syy += ydiff*ydiff;
            }
            double m = sxy/sxx;
            double b = yAvg - m*xAvg;
            LinearRegression regression = new LinearRegression(m, b);
            for (int i = 0; i < length; i++)
            {
                sserr += Math.Pow(xy[i, 1] - regression.GetY(xy[i, 0]), 2);
            }
            regression.RSquared = 1 - (sserr/syy);
            return regression;
        }

        public static LinearRegression Calculate(double[] x, double[] y)
        {
            int length = x.Length;
            if (length != y.Length)
            {
                throw new ArgumentException("X and Y Dimensions do not match");
            }
            double yAvg, sxy, sxx, syy, sserr;
            double xAvg = yAvg = sxy = sxx = syy = sserr = 0;

            for (int i = 0; i < length; i++)
            {
                xAvg += x[i];
                yAvg += y[i];
            }
            xAvg /= length;
            yAvg /= length;

            for (int i = 0; i < length; i++)
            {
                double xdiff = x[i] - xAvg;
                double ydiff = y[i] - yAvg;
                sxy += xdiff*ydiff;
                sxx += xdiff*xdiff;
                syy += ydiff*ydiff;
            }
            double m = sxy/sxx;
            double b = yAvg - m*xAvg;
            LinearRegression regression = new LinearRegression(m, b);
            for (int i = 0; i < length; i++)
            {
                sserr += Math.Pow(y[i] - regression.GetY(x[i]), 2);
            }
            regression.RSquared = 1 - (sserr/syy);
            return regression;
        }

        #endregion Static
    }
}