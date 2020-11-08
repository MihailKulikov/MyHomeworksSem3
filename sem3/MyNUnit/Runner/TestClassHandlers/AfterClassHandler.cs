using System;
using System.Threading.Tasks;

namespace MyNUnit.Runner.TestClassHandlers
{
    public class AfterClassHandler : TestClassHandler
    {
        public AfterClassHandler(TestClassHandler? nextHandlerIfHandleSuccess = null,
            TestClassHandler? nextHandlerIfHandleFailed = null) : base(nextHandlerIfHandleSuccess,
            nextHandlerIfHandleFailed)
        {
        }

        protected override bool RunMethods(TestResult testResult, TestClassWrapper testClass)
        {
            try
            {
                Parallel.ForEach(testClass.AfterClassMethodInfos,
                    afterClassMethod => afterClassMethod.Invoke(null, null));
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