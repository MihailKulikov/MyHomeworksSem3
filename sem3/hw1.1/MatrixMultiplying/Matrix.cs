using System;
using System.Threading.Tasks;

namespace MatrixMultiplying
{
    /// <summary>
    /// Represents a table of numbers. Provides methods to multiply matrices.
    /// </summary>
    public class Matrix
    {
        /// <summary>
        /// Elements of matrix.
        /// </summary>
        public int[,] Elements { get; }
        
        /// <summary>
        /// Initialize new instance of <see cref="Matrix"/> class with specified elements.
        /// </summary>
        /// <param name="elements">Specified elements of matrix.</param>
        /// <exception cref="ArgumentNullException"><paramref name="elements"/> is <code>null</code>.</exception>
        /// <exception cref="ArgumentException"><paramref name="elements"/> is empty.</exception>
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

        /// <summary>
        /// Returns result from multiplying this matrix with <paramref name="other"/> matrix.
        /// </summary>
        /// <param name="other">Second factor of matrix multiplying.</param>
        /// <returns>Result from multiplying this matrix with <paramref name="other"/> matrix.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <code>null</code>.</exception>
        /// <exception cref="ArgumentException">Row count of <paramref name="other"/> matrix is not equal to column count of this matrix.</exception>
        public Matrix MultiplyWith(Matrix other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (Elements.GetLength(1) != other.Elements.GetLength(0))
            {
                throw new ArgumentException($"Row count of {nameof(other)} matrix should be {Elements.GetLength(1)}.");
            }

            var resultMatrix = new int[Elements.GetLength(0), other.Elements.GetLength(1)];
            for (var currentRow = 0; currentRow < resultMatrix.GetLength(0); currentRow++)
            {
                for (var currentColumn = 0; currentColumn < resultMatrix.GetLength(1); currentColumn++)
                {
                    resultMatrix[currentRow, currentColumn] =
                        CalculateElementOfResultMatrix(currentRow, currentColumn, other);
                }
            }
            
            return new Matrix(resultMatrix);
        }

        private int CalculateElementOfResultMatrix(int rowNumber, int columnNumber, Matrix other)
        {
            var resultValue = 0;
            for (var i = 0; i < Elements.GetLength(1); i++)
            {
                resultValue += Elements[rowNumber, i] * other.Elements[i, columnNumber];
            }

            return resultValue;
        }

        public Matrix MultiplyWithParallel(Matrix other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (Elements.GetLength(1) != other.Elements.GetLength(0))
            {
                throw new ArgumentException($"Row count of {nameof(other)} matrix should be {Elements.GetLength(1)}.");
            }
            
            var resultMatrix = new int[Elements.GetLength(0), other.Elements.GetLength(1)];

            Parallel.For(0, resultMatrix.GetLength(0), currentRow =>
            {
                for (var currentColumn = 0; currentColumn < resultMatrix.GetLength(1); currentColumn++)
                {
                    resultMatrix[currentRow, currentColumn] =
                        CalculateElementOfResultMatrix(currentRow, currentColumn, other);
                }
            });

            return new Matrix(resultMatrix);
        }
    }
}