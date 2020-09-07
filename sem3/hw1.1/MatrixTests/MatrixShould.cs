using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MatrixMultiplying;
using NUnit.Framework;

namespace MatrixTests
{
    public class MatrixShould
    {
        private Matrix matrix;
        private const string FirstPath = "test1.txt";
        private const string SecondPath = "test2.txt";

        private static IEnumerable<(Matrix firstFactor, Matrix secondFactor, Matrix expectedResult)>
            ArgumentsForMultiplyingAndExpectedResultCases
        {
            get
            {
                yield return (new Matrix(new[,]
                {
                    {1, 1, 1},
                    {1, 1, 1},
                    {1, 1, 1}
                }), new Matrix(new[,]
                {
                    {1, 1, 1},
                    {1, 1, 1},
                    {1, 1, 1}
                }), new Matrix(new[,]
                {
                    {3, 3, 3},
                    {3, 3, 3},
                    {3, 3, 3}
                }));

                yield return (new Matrix(new[,]
                {
                    {4, 2}
                }), new Matrix(new[,]
                {
                    {5},
                    {4}
                }), new Matrix(new[,]
                {
                    {28}
                }));
            }
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            File.Delete(FirstPath);
            File.Delete(SecondPath);
        }
        
        [Test]
        public void Throw_ArgumentNullException_When_Initializing_With_Null()
        {
            Assert.That(() => new Matrix(null), Throws.ArgumentNullException);
        }

        [TestCase(0, 1)]
        [TestCase(1, 0)]
        [TestCase(0, 0)]
        [Test]
        public void Throw_ArgumentException_When_Initialing_With_Empty_Two_Dimensional_Array(int firstDimension, int secondDimension)
        {
            Assert.That(() => new Matrix(new int[firstDimension, secondDimension]), Throws.ArgumentException);
        }

        [Test]
        public void Initialize_Correct_Matrix_When_Two_Dimensional_Array_Correct()
        {
            var inputArray = new[,]
            {
                {1, 1},
                {1, 1},
                {1, 1},
                {1, 1}
            };
            matrix = new Matrix(inputArray);
            
            Assert.That(matrix.Elements, Is.EquivalentTo(inputArray));
        }

        [Test]
        public void Throw_ArgumentNullException_When_Trying_To_Multiply_Using_ParallelFor_With_Null()
        {
            matrix = new Matrix(new int[1,1]);
            
            Assert.That(() => matrix.MultiplyWithUsingParallelFor(null), Throws.ArgumentNullException);
        }

        [Test]
        public void Throw_ArgumentException_When_Trying_To_Multiply_Using_ParallelFor_With_Matrix_With_Wrong_Row_Count()
        {
            matrix = new Matrix(new int[1, 1]);

            Assert.That(() => matrix.MultiplyWithUsingParallelFor(new Matrix(new int[2, 1])), Throws.ArgumentException);
        }

        [TestCaseSource(nameof(ArgumentsForMultiplyingAndExpectedResultCases))]
        [Test]
        public void Return_Correct_Result_From_Multiplying_Using_ParallelFor_With_Other_Matrix(
            (Matrix firstFactor, Matrix secondFactor, Matrix result) argumentsForMultiplyingAndExpectedResultCase)
        {
            var (firstFactor, secondFactor, expectedResult) = argumentsForMultiplyingAndExpectedResultCase;

            var actualResult = firstFactor.MultiplyWithUsingParallelFor(secondFactor);

            Assert.That(actualResult.Elements, Is.EquivalentTo(expectedResult.Elements));
            Assert.That(actualResult.Elements.GetLength(0), Is.EqualTo(expectedResult.Elements.GetLength(0)));
            Assert.That(actualResult.Elements.GetLength(1), Is.EqualTo(expectedResult.Elements.GetLength(1)));
        }
        
        [Test]
        public void Throw_ArgumentNullException_When_Trying_To_Multiply_With_Null()
        {
            matrix = new Matrix(new int[1,1]);
            
            Assert.That(() => matrix.MultiplyWith(null), Throws.ArgumentNullException);
        }

        [Test]
        public void Throw_ArgumentException_When_Trying_To_Multiply_With_Matrix_With_Wrong_Row_Count()
        {
            matrix = new Matrix(new int[1, 1]);

            Assert.That(() => matrix.MultiplyWith(new Matrix(new int[2, 1])), Throws.ArgumentException);
        }

        [TestCaseSource(nameof(ArgumentsForMultiplyingAndExpectedResultCases))]
        [Test]
        public void Return_Correct_Result_From_Multiplying_With_Other_Matrix(
            (Matrix firstFactor, Matrix secondFactor, Matrix result) argumentsForMultiplyingAndExpectedResultCase)
        {
            var (firstFactor, secondFactor, expectedResult) = argumentsForMultiplyingAndExpectedResultCase;

            var actualResult = firstFactor.MultiplyWith(secondFactor);

            Assert.That(actualResult.Elements, Is.EquivalentTo(expectedResult.Elements));
            Assert.That(actualResult.Elements.GetLength(0), Is.EqualTo(expectedResult.Elements.GetLength(0)));
            Assert.That(actualResult.Elements.GetLength(1), Is.EqualTo(expectedResult.Elements.GetLength(1)));
        }

        [Test]
        public void Throw_ArgumentException_When_Trying_To_Generate_Random_Matrix_With_Non_Positive_Row_Count()
        {
            Assert.That(() => Matrix.GenerateRandomMatrix(-1, 10), Throws.ArgumentException);
        }

        [Test]
        public void Throw_ArgumentException_When_Trying_To_Generate_Random_Matrix_With_Non_Positive_Column_Count()
        {
            Assert.That(() => Matrix.GenerateRandomMatrix(10, -1), Throws.ArgumentException);
        }

        [Test]
        public void Generate_Random_Matrix()
        {
            var generatedMatrix = Matrix.GenerateRandomMatrix(42, 2);

            Assert.That(generatedMatrix.Elements.GetLength(0), Is.EqualTo(42));
            Assert.That(generatedMatrix.Elements.GetLength(1), Is.EqualTo(2));
        }

        [Test]
        public void Throw_ArgumentNullException_When_Trying_To_Read_Matrix_From_Null_Path()
        {
            Assert.That(() => Matrix.ReadMatrixFromFileAsync(null), Throws.ArgumentNullException);
        }

        [Test]
        public void Throw_ArgumentException_When_Trying_To_Read_Matrix_From_File_That_Does_Not_Exist()
        {
            Assert.That(() => Matrix.ReadMatrixFromFileAsync(""),
                Throws.ArgumentException);
        }

        [Test]
        public void Throw_ArgumentException_When_Trying_To_Read_Matrix_From_Empty_File()
        {
            File.WriteAllText(FirstPath, "");
            const string exceptionMessage = "File is empty.";
            
            Assert.That(() => Matrix.ReadMatrixFromFileAsync(FirstPath),
                Throws.ArgumentException.And.Message.EqualTo(exceptionMessage));
        }

        [Test]
        public void Throw_ArgumentException_When_Trying_To_Read_Incorrect_Matrix_From_File()
        {
            File.WriteAllLines(FirstPath, new[]{"4 2", "1"});
            const string exceptionMessage = "Matrix has incorrect format.";

            Assert.That(() => Matrix.ReadMatrixFromFileAsync(FirstPath),
                Throws.ArgumentException.And.Message.EqualTo(exceptionMessage));
        }

        [Test]
        public async Task Read_Matrix_From_File()
        {
            var rows = new[] {"4 2", "5 4", "1 1"};
            File.WriteAllLines(FirstPath,rows);

            var result = await Matrix.ReadMatrixFromFileAsync(FirstPath);
            
            Assert.That(result.Elements,
                Is.EquivalentTo(rows.SelectMany(row => row.Split(' ').Select(int.Parse))));
            Assert.That(result.Elements.GetLength(0), Is.EqualTo(rows.Length));
            Assert.That(result.Elements.GetLength(1), Is.EqualTo(2));
        }

        [Test]
        public async Task Write_Matrix_To_File()
        {
            matrix = new Matrix(new[,]
            {
                {4, 2},
                {5, 4},
                {6, 6}
            });
            var expectedResult = new[] {"4 2", "5 4", "6 6"};

            var path = await matrix.WriteMatrixToNewFileAsync();

            Assert.That(await File.ReadAllLinesAsync(path), Is.EquivalentTo(expectedResult));
            
            File.Delete(path);
        }

        [Test]
        public async Task Read_Two_Matrices_From_File_And_Write_Result_From_Multiplying_To_New_File()
        {
            var firstMatrixRows = new[] {"4 2", "5 4", "1 1"};
            File.WriteAllLines(FirstPath, firstMatrixRows);
            var secondMatrixRows = new[] {"1", "1"};
            File.WriteAllLines(SecondPath, secondMatrixRows);
            var expectedResult = new[] {"6", "9", "2"};

            var path = await Matrix.ReadTwoMatricesFromFileAndWriteResultFromMultiplyingToNewFileAsync(FirstPath,
                SecondPath);

            Assert.That(await File.ReadAllLinesAsync(path), Is.EquivalentTo(expectedResult));
            
            File.Delete(path);
        }
        
        [Test]
        public void Throw_ArgumentNullException_When_Trying_To_Multiply_Using_Threads_With_Null()
        {
            matrix = new Matrix(new int[1,1]);
            
            Assert.That(() => matrix.ParallelMultiplyWithUsingThreads(null), Throws.ArgumentNullException);
        }

        [Test]
        public void Throw_ArgumentException_When_Trying_To_Multiply_Using_Threads_With_Matrix_With_Wrong_Row_Count()
        {
            matrix = new Matrix(new int[1, 1]);

            Assert.That(() => matrix.ParallelMultiplyWithUsingThreads(new Matrix(new int[2, 1])), Throws.ArgumentException);
        }

        [TestCaseSource(nameof(ArgumentsForMultiplyingAndExpectedResultCases))]
        [Test]
        public void Return_Correct_Result_From_Multiplying_Using_Threads_With_Other_Matrix(
            (Matrix firstFactor, Matrix secondFactor, Matrix result) argumentsForMultiplyingAndExpectedResultCase)
        {
            var (firstFactor, secondFactor, expectedResult) = argumentsForMultiplyingAndExpectedResultCase;

            var actualResult = firstFactor.MultiplyWith(secondFactor);

            Assert.That(actualResult.Elements, Is.EquivalentTo(expectedResult.Elements));
            Assert.That(actualResult.Elements.GetLength(0), Is.EqualTo(expectedResult.Elements.GetLength(0)));
            Assert.That(actualResult.Elements.GetLength(1), Is.EqualTo(expectedResult.Elements.GetLength(1)));
        }
    }
}