using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public double[,] GetMatrix()
        {
            return _purityMatrix.Storage.ToArray();
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
