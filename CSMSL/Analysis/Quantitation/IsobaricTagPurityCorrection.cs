using MathNet.Numerics.LinearAlgebra.Double;

namespace CSMSL.Analysis.Quantitation
{
    public class IsobaricTagPurityCorrection
    {
        private readonly Matrix _purityMatrix;       

        private IsobaricTagPurityCorrection(Matrix matrix)
        {
            _purityMatrix = matrix;    
        }            

        public double[] ApplyPurityCorrection(double[] rawData)
        {
            return _purityMatrix.LU().Solve(new DenseVector(rawData)).ToArray();
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
