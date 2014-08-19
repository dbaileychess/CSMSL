// Copyright 2012, 2013, 2014 Derek J. Bailey
// 
// This file (IsobaricTagPurityCorrection.cs) is part of CSMSL.
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
using System.Collections.Generic;
using System.Linq;

namespace CSMSL.Analysis.Quantitation
{
    public class IsobaricTagPurityCorrection
    {
        //private readonly Matrix _purityMatrix;

        private readonly double[,] _purityMatrix;
        private readonly double[,] _purityLUMatrix;
        private readonly int[] _purityLUMatrixIndex;

        private IsobaricTagPurityCorrection(double[,] matrix)
        {
            _purityMatrix = matrix;
            int[] index;
            _purityLUMatrix = _purityMatrix.LUDecompose(out index);
            _purityLUMatrixIndex = index;
        }

        public double[,] GetMatrix()
        {
            return _purityMatrix.Copy();
            //return _purityMatrix;
        }

        //public double Determinant()
        //{
        //    return _purityMatrix.Determinant();
        //}

        public double[] ApplyPurityCorrection(IEnumerable<double> rawData)
        {
            return ApplyPurityCorrection(rawData.ToArray());
        }

        public double[] ApplyPurityCorrection(double[] rawData)
        {
            if (rawData.Length != _purityLUMatrix.GetLength(0))
            {
                throw new ArgumentException("Not enough data points");
            }
            double[] result = _purityLUMatrix.Solve(rawData, _purityLUMatrixIndex);
            return result;
        }

        /// <summary>
        /// Creates a matrix with the solutions to isobaric purity corrections
        /// </summary>
        /// <param name="purityValues"></param>
        /// <returns></returns>
        public static IsobaricTagPurityCorrection Create(double[,] purityValues)
        {
            int rows = purityValues.GetLength(0);
            int inputCount = purityValues.GetLength(1);

            double[,] purityMatrix = new double[rows, rows];

            //w x y z part of iTracker Paper
            for (int i = 0; i < rows; i++)
            {
                double startvalue = 100;
                for (int j = 0; j < inputCount; j++)
                {
                    startvalue -= purityValues[i, j];
                }

                for (int j = 0; j < rows; j++)
                {
                    if (j == i) continue; // Handled in the above code
                    double value = 0; // Zero fill;

                    int k = (j > i) ? 2 - j + i : i - j + 1;

                    if (k < inputCount && k >= 0)
                        value = purityValues[j, k];

                    purityMatrix[i, j] = value;
                }

                purityMatrix[i, i] = startvalue;
            }

            return new IsobaricTagPurityCorrection(purityMatrix);
        }
    }
}