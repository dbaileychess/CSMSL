// Copyright 2022 Derek J. Bailey
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

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