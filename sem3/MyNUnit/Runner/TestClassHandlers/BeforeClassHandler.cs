using System;
using System.Threading.Tasks;

namespace MyNUnit.Runner.TestClassHandlers
{
    public class BeforeClassHandler : TestClassHandler
    {
        public BeforeClassHandler(TestClassHandler? nextHandlerIfHandleSuccess = null,
            TestClassHandler? nextHandlerIfHandleFailed = null) : base(nextHandlerIfHandleSuccess,
            nextHandlerIfHandleFailed)
        {
        }

        protected override bool RunMethods(TestResult testResult, TestClassWrapper testClass)
        {
            try
            {
                Parallel.ForEach(testClass.BeforeClassMethodInfos,
                    beforeClassMethod => beforeClassMethod.Invoke(null, null));
            }
            catch (Exception e)
            {
                testResult.Exceptions.Enqueue(e);
                return false;
            }

            return true;
        }
    }
}