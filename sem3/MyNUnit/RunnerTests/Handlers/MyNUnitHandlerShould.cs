using System;
using System.Collections.Generic;
using Moq;
using MyNUnit.Runner.TestClassHandlers;
using NUnit.Framework;

namespace RunnerTests.Handlers
{
    public class MyNUnitHandlerShould
    {
        private MyNUnitHandler handler;

        private static IEnumerable<Func<MyNUnitHandler>> FactoryWithoutArguments
        {
            get
            {
                yield return () => new AfterClassHandler();
                yield return () => new AfterHandler(null);
                yield return () => new BeforeHandler(null);
                yield return () => new BeforeClassHandler();
                yield return () => new TestHandler(null);
                yield return () => new MultipleBeforeTestAfterHandler();
            }
        }

        private static IEnumerable<Func<MyNUnitHandler, MyNUnitHandler, MyNUnitHandler>> FactoryWithArguments
        {
            get
            {
                yield return (success, fail)
                    => new AfterClassHandler(success, fail);
                yield return (success, fail)
                    => new AfterHandler(success, fail);
                yield return (success, fail)
                    => new BeforeHandler(success, fail);
                yield return (success, fail)
                    => new BeforeClassHandler(success, fail);
                yield return (success, fail)
                    => new TestHandler(success, fail);
                yield return (success, fail)
                    => new MultipleBeforeTestAfterHandler(success, fail);
            }
        }

        [TestCaseSource(nameof(FactoryWithoutArguments))]
        [Test]
        public void Have_Null_Next_Handlers_After_Creation_Without_Arguments(Func<MyNUnitHandler> factory)
        {
            handler = factory.Invoke();

            Assert.That(handler.NextHandlerIfHandlingFailed, Is.Null);
            Assert.That(handler.NextHandlerIfHandlingWasSuccessful, Is.Null);
        }

        [TestCaseSource(nameof(FactoryWithArguments))]
        [Test]
        public void Have_Specified_Next_Handlers_After_Creation_With_Specified_Arguments(
            Func<MyNUnitHandler, MyNUnitHandler, MyNUnitHandler> factory)
        {
            var successfulNextHandler = It.IsAny<MyNUnitHandler>();
            var failedNextHandler = It.IsAny<MyNUnitHandler>();

            handler = factory.Invoke(successfulNextHandler, failedNextHandler);

            Assert.That(handler.NextHandlerIfHandlingWasSuccessful, Is.EqualTo(successfulNextHandler));
            Assert.That(handler.NextHandlerIfHandlingFailed, Is.EqualTo(failedNextHandler));
        }
    }
}