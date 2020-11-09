using System;
using System.Threading.Tasks;

namespace MyNUnit.Runner.TestClassHandlers
{
    public class AfterHandler : MyNUnitHandler
    {
        public AfterHandler(MyNUnitHandler? nextHandlerIfHandleSuccess = null,
            MyNUnitHandler? nextHandlerIfHandleFailed = null) :
            base(nextHandlerIfHandleSuccess, nextHandlerIfHandleFailed)
        {
        }

        protected override bool RunMethods(TestResult testResult, TestClassWrapper testClass)
        {
            try
            {
                Parallel.ForEach(testClass.AfterMethodInfos,
                    afterMethod => afterMethod.Invoke(testClass.TestClassInstance, null));
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