using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra.Double;

namespace CSMSL.Analysis.Quantitation
{
    public class IsobaricTagPurityCorrection
    {
        private readonly Matrix _purityMatrix;
        private int _rows;

        private IsobaricTagPurityCorrection(Matrix matrix)
        {
            _purityMatrix = matrix;
            _rows = matrix.RowCount;
        }

        public double Determinant()
        {
            return _purityMatrix.Determinant();
        }

        public double[] ApplyPurityCorrection(IEnumerable<double> rawData)
        {
            return ApplyPurityCorrection(rawData.ToArray());
        }

        public double[] ApplyPurityCorrection(double[] rawData)
        {
            if (rawData.Length != _rows)
            {
                throw new ArgumentException("Not enough data points");
            }
            return _purityMatrix.LU().Solve(new DenseVector(rawData)).ToArray();
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

            Matrix purityMatrix = new DenseMatrix(rows);
           
            // w x y z part of iTracker Paper
            for (int i = 0; i < rows; i++)
            {
                purityMatrix[i, i] = 1.0;
                for (int j = 0; j < inputCount; j++)
                {
                    purityMatrix[i, i] -= purityValues[i, j] / 100;
                }
            }

            // Setting up the C matrix
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    if (j == i) continue; // Handled in the above code                  
                    int k = (j > i) ? 3 - j + i : 2 - j + i;
                    purityMatrix[i, j] = (k < inputCount && k > 0) ? purityValues[j, k] / 100 : 0; // Zero fill missing values
                }
            }

            return new IsobaricTagPurityCorrection(purityMatrix);
        }

    }
}
