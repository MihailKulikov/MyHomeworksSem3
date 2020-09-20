using System.Collections.Generic;
using LazyInitialization;
using Moq;
using NUnit.Framework;

namespace LazyInitializationTests
{
    public class LazyShould
    {
        public interface IObjectFactory
        {
            object GetNewObject();
        }

        private Lazy<object> lazy;
        private Mock<IObjectFactory> objectFactoryMock;

        private static IEnumerable<object> ExpectedObjectCases
        {
            get
            {
                yield return null;
                yield return new object();
            }
        }
        
        [SetUp]
        public void SetUp()
        {
            objectFactoryMock = new Mock<IObjectFactory>();
        }

        [Test]
        public void Throw_ArgumentNullException_When_Trying_To_Initialize_With_Null_Supplier()
        {
            Assert.That(() => new Lazy<object>(null), Throws.ArgumentNullException);
        }
        
        [TestCaseSource(nameof(ExpectedObjectCases))]
        [Test]
        public void Calculate_And_Return_Value_For_The_First_Call_Get_Method(object expectedObject)
        {
            lazy = new Lazy<object>(() => objectFactoryMock.Object.GetNewObject());
            objectFactoryMock.Setup(factory => factory.GetNewObject()).Returns(expectedObject);

            var actualObject = lazy.Get();
            Assert.That(actualObject, Is.EqualTo(expectedObject));
            objectFactoryMock.Verify(factory => factory.GetNewObject(), Times.Once);
        }

        [TestCaseSource(nameof(ExpectedObjectCases))]
        [Test]
        public void Returns_The_Same_Object_For_The_Second_Call_Get_Method(object expectedObject)
        {
            lazy = new Lazy<object>(() => objectFactoryMock.Object.GetNewObject());
            objectFactoryMock.Setup(factory => factory.GetNewObject()).Returns(expectedObject);

            var firstObject = lazy.Get();
            var secondObject = lazy.Get();
            
            Assert.That(firstObject, Is.EqualTo(secondObject));
            Assert.That(firstObject, Is.EqualTo(expectedObject));
            objectFactoryMock.Verify(factory => factory.GetNewObject(), Times.Once);
        }

        [Test]
        public void Throw_InvalidOperationException_When_The_Initialization_Function_Tries_To_Call_Method_Get_Of_This_Instance()
        {
            const string exceptionMessage = "Recursive calls Get().";
            lazy = new Lazy<object>(() => new object());
            lazy = new Lazy<object>(() => lazy.Get());
            
            Assert.That(() => lazy.Get(), Throws.InvalidOperationException.And.Message.EqualTo(exceptionMessage));
        }
    }
}