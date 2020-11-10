using System;
using MyNUnit.Attributes;

namespace TestProject
{
    public class TestClass
    {
        [Test]
        public void TestMe()
        {
            Console.WriteLine("Hello, boi!");
        }

        [Test]
        public void ThrowException()
        {
            throw new Exception("haha");
        }

        [Test(Expected = typeof(AggregateException))]
        public void ThrowUnexpectedException()
        {
            throw new ArgumentException();
        }
        
        [Test(Expected = typeof(Exception))]
        public void ThrowExpectedException()
        {
            throw new Exception("Haha");
        }

        [Test(Ignore = "NOT")]
        public void DontTestMe()
        {
            //
        }
    }

    public class AnotherOne
    {
        [Test]
        public void TestMe()
        {
        }
    }
}