using System;

namespace MatrixMultiplying
{
    public class Matrix
    {
        public Matrix(int[,] elements)
        {
            if (elements == null)
            {
                throw new ArgumentNullException(nameof(elements));
            }
            
            if (elements.GetLength(0) == 0 || elements.GetLength(1) == 0)
            {
                throw new ArgumentException($"{nameof(elements)} should not be empty.");
            }

            Elements = elements;
        }
        
        public int[,] Elements { get; }

        public Matrix MultiplyWith(Matrix other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (other.Elements.GetLength(0) == 0 || other.Elements.GetLength(1) == 0)
            {
                throw new ArgumentException($"{nameof(other)} should not be empty.");
            }

            if (Elements.GetLength(1) == other.Elements.GetLength(0))
            {
                throw new ArgumentException($"Row count of {nameof(other)} matrix should be {Elements.GetLength(1)}.");
            }

            var resultMatrix = new int[Elements.GetLength(0), other.Elements.GetLength(1)];
            for (var currentRow = 0; currentRow < resultMatrix.GetLength(0); currentRow++)
            {
                for (var currentColumn = 0; currentColumn < resultMatrix.GetLength(1); currentColumn++)
                {
                    for (var i = 0; i < Elements.GetLength(1); i++)
                    {
                        resultMatrix[currentRow, currentColumn] +=
                            Elements[currentRow, i] * other.Elements[i, currentColumn];
                    }
                }
            }
            
            return new Matrix(resultMatrix);
        }
        
        // public Matrix MultiplyWithAsync(Matrix other)
        // {}
    }
}