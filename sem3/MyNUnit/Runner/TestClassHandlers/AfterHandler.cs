using System;
using System.Threading.Tasks;

namespace MyNUnit.Runner.TestClassHandlers
{
    public class AfterHandler : TestClassHandler
    {
        public AfterHandler(TestClassHandler? nextHandlerIfHandleSuccess = null,
            TestClassHandler? nextHandlerIfHandleFailed = null) :
            base(nextHandlerIfHandleSuccess, nextHandlerIfHandleFailed)
        {
        }

        protected override bool RunMethods(TestResult testResult, TestClassWrapper testClass)
        {
            try
            {
                Parallel.ForEach(testClass.BeforeMethodInfos,
                    beforeMethod => beforeMethod.Invoke(testClass.TestClassInstance, null));
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