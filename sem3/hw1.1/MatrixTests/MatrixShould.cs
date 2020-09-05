using System.Collections.Generic;
using MatrixMultiplying;
using NUnit.Framework;

namespace MatrixTests
{
    public class MatrixShould
    {
        private Matrix matrix;

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
        public void Throw_ArgumentNullException_When_Trying_To_MultiplyParallel_With_Null()
        {
            matrix = new Matrix(new int[1,1]);
            
            Assert.That(() => matrix.ParallelMultiplyWith(null), Throws.ArgumentNullException);
        }

        [Test]
        public void Throw_ArgumentException_When_Trying_To_MultiplyParallel_With_Matrix_With_Wrong_Row_Count()
        {
            matrix = new Matrix(new int[1, 1]);

            Assert.That(() => matrix.ParallelMultiplyWith(new Matrix(new int[2, 1])), Throws.ArgumentException);
        }

        [TestCaseSource(nameof(ArgumentsForMultiplyingAndExpectedResultCases))]
        [Test]
        public void Return_Correct_Result_From_MultiplyingParallel_With_Other_Matrix(
            (Matrix firstFactor, Matrix secondFactor, Matrix result) argumentsForMultiplyingAndExpectedResultCase)
        {
            var (firstFactor, secondFactor, expectedResult) = argumentsForMultiplyingAndExpectedResultCase;

            var actualResult = firstFactor.ParallelMultiplyWith(secondFactor);

            Assert.That(actualResult.Elements, Is.EquivalentTo(expectedResult.Elements));
        }
        
        [Test]
        public void Throw_ArgumentNullException_When_Trying_To_Multiply_With_Null()
        {
            matrix = new Matrix(new int[1,1]);
            
            Assert.That(() => matrix.ParallelMultiplyWith(null), Throws.ArgumentNullException);
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
            var (firstFactor, secondFactor, expectedResult) = (
                argumentsForMultiplyingAndExpectedResultCase.firstFactor,
                argumentsForMultiplyingAndExpectedResultCase.secondFactor,
                argumentsForMultiplyingAndExpectedResultCase.result);

            var actualResult = firstFactor.MultiplyWith(secondFactor);

            Assert.That(actualResult.Elements, Is.EquivalentTo(expectedResult.Elements));
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

            Assert.That(generatedMatrix.Elements.GetLength(0) == 42);
            Assert.That(generatedMatrix.Elements.GetLength(1) == 2);
        }
    }
}