using MatrixMultiplying;
using NUnit.Framework;

namespace MatrixTests
{
    public class Tests
    {
        private Matrix matrix;

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ThrowArgumentNullExceptionWhenInitializingWithNull()
        {
            Assert.That(() => new Matrix(null), Throws.ArgumentNullException);
        }

        [TestCase(0, 1)]
        [TestCase(1, 0)]
        [TestCase(0, 0)]
        [Test]
        public void ThrowArgumentExceptionWhenInitialingWithEmptyTwoDimensionalArray(int firstDimension, int secondDimension)
        {
            Assert.That(() => new Matrix(new int[firstDimension, secondDimension]), Throws.ArgumentException);
        }

        [Test]
        public void InitializeCorrectMatrixWhenTwoDimensionalArrayCorrect()
        {
            var inputArray = new int[4, 2]
            {
                {1, 1},
                {1, 1},
                {1, 1},
                {1, 1}
            };
            matrix = new Matrix(inputArray);
            
            Assert.That(matrix.Elements, Is.EquivalentTo(inputArray));
        }
    }
}