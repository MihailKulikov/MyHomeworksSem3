using System;
using System.Collections.Generic;
using LazyInitialization;
using Moq;
using NUnit.Framework;

namespace LazyInitializationTests
{
    public enum LazyType
    {
        NotThreadSafeLazy,
        ThreadSafeLazy
    }
    
    [TestFixture(LazyType.NotThreadSafeLazy)]
    [TestFixture(LazyType.ThreadSafeLazy)]
    public class SingleThreadedTests
    {
        public interface IObjectFactory
        {
            object GetNewObject();
        }
        
        private ILazy<object> lazy;
        private Mock<IObjectFactory> objectFactoryMock;
        private readonly Func<Func<object>, ILazy<object>> initializer;

        private static IEnumerable<object> ExpectedObjectCases
        {
            get
            {
                yield return null;
                yield return new object();
            }
        }

        public SingleThreadedTests(LazyType lazyType)
        {
            initializer = lazyType switch
            {
                LazyType.NotThreadSafeLazy => supplier => new NotThreadSafeLazy<object>(supplier),
                LazyType.ThreadSafeLazy => supplier => new ThreadSafeLazy<object>(supplier),
                _ => throw new ArgumentException("Unadded case.")
            };
        }
        
        [SetUp]
        public void SetUp()
        {
            objectFactoryMock = new Mock<IObjectFactory>();
        }

        [Test]
        public void Throw_ArgumentNullException_When_Trying_To_Initialize_With_Null_Supplier()
        {
            Assert.That(() => initializer(null), Throws.ArgumentNullException);
        }
        
        [TestCaseSource(nameof(ExpectedObjectCases))]
        [Test]
        public void Calculate_And_Return_Value_For_The_First_Call_Get_Method(object expectedObject)
        {
            lazy = initializer(() => objectFactoryMock.Object.GetNewObject());
            objectFactoryMock.Setup(factory => factory.GetNewObject()).Returns(expectedObject);

            var actualObject = lazy.Get();
            Assert.That(actualObject, Is.EqualTo(expectedObject));
            objectFactoryMock.Verify(factory => factory.GetNewObject(), Times.Once);
        }

        [TestCaseSource(nameof(ExpectedObjectCases))]
        [Test]
        public void Returns_The_Same_Object_For_The_Second_Call_Get_Method(object expectedObject)
        {
            lazy = initializer(() => objectFactoryMock.Object.GetNewObject());
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
            lazy = initializer(() => new object());
            lazy = initializer(() => lazy.Get());
            
            Assert.That(() => lazy.Get(), Throws.InvalidOperationException.And.Message.EqualTo(exceptionMessage));
        }
    }
}