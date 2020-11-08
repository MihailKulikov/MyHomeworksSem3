namespace MyNUnit.Runner.TestClassHandlers
{
    public abstract class TestClassHandler
    {
        public TestClassHandler? NextHandlerIfHandleSuccess { get; set; }
        public TestClassHandler? NextHandlerIfHandleFailed { get; set; }

        protected TestClassHandler(TestClassHandler? nextHandlerIfHandleSuccess = null,
            TestClassHandler? nextHandlerIfHandleFailed = null)
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