using MyNUnit.Attributes;
using MyNUnit.Runner;
using MyNUnit.Runner.Interfaces;
using NUnit.Framework;

namespace RunnerTests
{
    public class TestClassWrapperShould
    {
        private class ForTest
        {
            [After]
            public void After()
            {
            }

            [AfterClass]
            public void AfterClassNotStatic()
            {
            }

            [AfterClass]
            public static void AfterClassStatic()
            {
            }

            [Before]
            public void Before()
            {
            }

            [BeforeClass]
            public void BeforeClassNotStatic()
            {
            }

            [BeforeClass]
            public static void BeforeClassStatic()
            {
            }

            [MyNUnit.Attributes.Test]
            public void Test()
            {
            }

            public void SimpleMethod()
            {
            }
        }

        private ITestClassWrapper testClassWrapper;

        [NUnit.Framework.Test]
        public void Initialize_Correctly()
        {
            testClassWrapper = new TestClassWrapper(typeof(ForTest));

            Assert.That(testClassWrapper.ClassType, Is.EqualTo(typeof(ForTest)));
            Assert.That(testClassWrapper.AfterMethodInfos,
                Is.EquivalentTo(new[] {typeof(ForTest).GetMethod(nameof(ForTest.After))}));
            Assert.That(testClassWrapper.BeforeMethodInfos,
                Is.EquivalentTo(new[] {typeof(ForTest).GetMethod(nameof(ForTest.Before))}));
            Assert.That(testClassWrapper.TestClassInstance, Is.TypeOf<ForTest>());
            Assert.That(testClassWrapper.TestMethodInfos,
                Is.EquivalentTo(new[] {typeof(ForTest).GetMethod(nameof(ForTest.Test))}));
            Assert.That(testClassWrapper.AfterClassMethodInfos,
                Is.EquivalentTo(new[] {typeof(ForTest).GetMethod(nameof(ForTest.AfterClassStatic))}));
            Assert.That(testClassWrapper.BeforeClassMethodInfos,
                Is.EquivalentTo(new[] {typeof(ForTest).GetMethod(nameof(ForTest.BeforeClassStatic))}));
        }
    }
}