namespace MyNUnit.Runner.TestClassHandlers
{
    public abstract class MyNUnitHandler
    {
        public MyNUnitHandler? NextHandlerIfHandleSuccess { get; set; }
        public MyNUnitHandler? NextHandlerIfHandleFailed { get; set; }

        protected MyNUnitHandler(MyNUnitHandler? nextHandlerIfHandleSuccess = null,
            MyNUnitHandler? nextHandlerIfHandleFailed = null)
        {
            NextHandlerIfHandleSuccess = nextHandlerIfHandleSuccess;
            NextHandlerIfHandleFailed = nextHandlerIfHandleFailed;
        }

        public TestResult Handle(TestResult testResult, TestClassWrapper testClass)
        {
            return RunMethods(testResult, testClass)
                ? NextHandlerIfHandleSuccess?.Handle(testResult, testClass) ?? testResult
                : NextHandlerIfHandleFailed?.Handle(testResult, testClass) ?? testResult;
        }

        protected abstract bool RunMethods(TestResult testResult, TestClassWrapper testClass);
    }
}