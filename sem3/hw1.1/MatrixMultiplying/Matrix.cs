using System;
using System.IO;
using System.Linq;
using System.Threading;
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
        /// Calculates result from multiplying this matrix with <paramref name="other"/> matrix.
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
            CalculateSpecifiedElementsOfResultMatrix(resultMatrix, 0, 0, 1, 1, other);

            return new Matrix(resultMatrix);
        }
        
        /// <summary>
        /// Calculates result from multiplying this matrix with <paramref name="other"/> matrix with using multiple threads.
        /// </summary>
        /// <param name="other">Second factor of matrix multiplying.</param>
        /// <returns>Result from multiplying this matrix with <paramref name="other"/> matrix.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <code>null</code>.</exception>
        /// <exception cref="ArgumentException">Row count of <paramref name="other"/> matrix is not equal to column count of this matrix.</exception>
        public Matrix ParallelForMultiplyWith(Matrix other)
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

        /// <summary>
        /// Calculates result from multiplying this matrix with <paramref name="other"/> matrix with using multiple threads.
        /// </summary>
        /// <param name="other">Second factor of matrix multiplying.</param>
        /// <returns>Result from multiplying this matrix with <paramref name="other"/> matrix.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <code>null</code>.</exception>
        /// <exception cref="ArgumentException">Row count of <paramref name="other"/> matrix is not equal to column count of this matrix.</exception>
        public Matrix MyVersionOfParallelMultiplyWith(Matrix other)
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
            var maxSide = Math.Max(resultMatrix.GetLength(0), resultMatrix.GetLength(1));
            var threads = new Thread[Math.Min(Environment.ProcessorCount, maxSide)];

            for (var currentThread = 0; currentThread < threads.Length; currentThread++)
            {
                var localCurrentThread = currentThread;
                if (resultMatrix.GetLength(0) == maxSide)
                {
                    threads[currentThread] = new Thread(() =>
                    {
                        CalculateSpecifiedElementsOfResultMatrix(resultMatrix, localCurrentThread, 0, threads.Length, 1,
                            other);
                    });
                }
                else
                {
                    threads[currentThread] = new Thread(() =>
                    {
                        CalculateSpecifiedElementsOfResultMatrix(resultMatrix, 0, localCurrentThread, 1, threads.Length,
                            other);
                    });
                }
                
                threads[currentThread].Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }
            
            return new Matrix(resultMatrix);
        }

        /// <summary>
        /// Generate new instance of <see cref="Matrix"/> class with random elements and specified row's and column's count.
        /// </summary>
        /// <param name="rowCount">Specified row's count of generated matrix.</param>
        /// <param name="columnCount">Specified column's count of generated matrix.</param>
        /// <returns>Generated matrix with random elements and specified row's and column's count</returns>
        /// <exception cref="ArgumentException"><paramref name="rowCount"/> is not positive; <paramref name="columnCount"/> is not positive.</exception>
        public static Matrix GenerateRandomMatrix(int rowCount, int columnCount)
        {
            if (rowCount <= 0)
            {
                throw new ArgumentException($"{nameof(rowCount)} should be positive number.");
            }

            if (columnCount <= 0)
            {
                throw new ArgumentException($"{nameof(columnCount)} should be positive number.");
            }

            var matrix = new int[rowCount, columnCount];
            var random = new Random();

            for (var i = 0; i < rowCount; i++)
            {
                for (var j = 0; j < columnCount; j++)
                {
                    matrix[i, j] = random.Next(int.MinValue, int.MaxValue);
                }
            }
            
            return new Matrix(matrix);
        }

        /// <summary>
        /// Asynchronously opens a text file, reads matrix from the file, and then closes the file.
        /// </summary>
        /// <param name="path">The file to open for reading.</param>
        /// <returns>A matrix with elements from the file.</returns>
        /// <exception cref="ArgumentException">File is empty; matrix has incorrect format.</exception>
        public static async Task<Matrix> ReadMatrixFromFileAsync(string path)
        {
            var rows = await File.ReadAllLinesAsync(path);

            if (rows.Length == 0)
            {
                throw new ArgumentException("File is empty.");
            }

            var matrixFromFile = rows.Select(row => row.Split(' ').Select(int.Parse).ToArray()).ToArray();
            var expectedColumnCount = matrixFromFile[0].Length;

            if (matrixFromFile.Any(row => row.Length != expectedColumnCount))
            {
                throw new ArgumentException("Matrix has incorrect format.");
            }

            var matrix = new int[matrixFromFile.Length, expectedColumnCount];
            for (var i = 0; i < matrix.GetLength(0); i++)
            {
                for (var j = 0; j < matrix.GetLength(1); j++)
                {
                    matrix[i, j] = matrixFromFile[i][j];
                }
            }
            
            return new Matrix(matrix);
        }

        /// <summary>
        /// Asynchronously creates a new file, writes the matrix to the file, and then closes the file.
        /// </summary>
        /// <returns>The file's path.</returns>
        public async Task<string> WriteMatrixToNewFileAsync()
        {
            var fileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ".txt";

            var rows = new string[Elements.GetLength(0)];
            for (var i = 0; i < Elements.GetLength(0); i++)
            {
                for (var j = 0; j < Elements.GetLength(1) - 1; j++)
                {
                    rows[i] += Elements[i, j] + " ";
                }

                rows[i] += Elements[i, Elements.GetLength(1) - 1];
            }

            await File.WriteAllLinesAsync(fileName, rows);

            return fileName;
        }

        /// <summary>
        /// Asynchronously opens two text files, reads matrices from the files, multiply them, writes result to the new file, and then closes all files.
        /// </summary>
        /// <param name="firstPath">The first file to open for reading.</param>
        /// <param name="secondPath">The second file to open for reading.</param>
        /// <returns>The path of file with result of matrix multiplication.</returns>
        public static async Task<string> ReadTwoMatricesFromFileAndWriteResultFromMultiplyingToNewFileAsync(string firstPath,
            string secondPath)
        {
            var firstMatrix = await ReadMatrixFromFileAsync(firstPath);
            var secondMatrix = await ReadMatrixFromFileAsync(secondPath);

            return await firstMatrix.MyVersionOfParallelMultiplyWith(secondMatrix).WriteMatrixToNewFileAsync();
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
        
        private void CalculateSpecifiedElementsOfResultMatrix(int[,] resultMatrix, int startRow, int startColumn,
            int rowStep, int columnStep, Matrix other)
        {
            for (var currentRow = startRow; currentRow < resultMatrix.GetLength(0); currentRow+=rowStep)
            {
                for (var currentColumn = startColumn; currentColumn < resultMatrix.GetLength(1); currentColumn+=columnStep)
                {
                    resultMatrix[currentRow, currentColumn] =
                        CalculateElementOfResultMatrix(currentRow, currentColumn, other);
                }
            }
        }
    }
}