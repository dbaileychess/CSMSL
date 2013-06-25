using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Double.Solvers.Iterative;

namespace CSMSL.Analysis.Quantitation
{
    public class IsobaricTagPurityCorrection
    {

        private Matrix _purityMatrix;       

        private IsobaricTagPurityCorrection(Matrix matrix)
        {
            _purityMatrix = matrix;    
           
        }            

        public double[] ApplyPurityCorrection(double[] rawData)
        {
            return _purityMatrix.LU().Solve(new DenseVector(rawData)).ToArray();
            //return _solver.Solve(_purityMatrix, new DenseVector(rawData)).ToArray();              
        }

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
            int k = 0;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    if (j == i) continue; // Handled in the above code                  
                    k = (j > i) ? 3 - j + i : 2 - j + i;  // the k index changes based on if you are on the upper or lower diagonal     
                    purityMatrix[i, j] = (k < inputCount && k > 0) ? purityValues[j, k] / 100 : 0; // Zero fill missing values
                }
            }

            return new IsobaricTagPurityCorrection(purityMatrix);
        }

    }
}
