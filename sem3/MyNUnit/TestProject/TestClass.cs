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
        
        [Test(Expected = typeof(Exception))]
        public void MeToo()
        {
            throw new Exception("Haha");
        }

        [Test(Ignore = "NOT")]
        public void DontTestMe()
        {
            //
        }
    }
}