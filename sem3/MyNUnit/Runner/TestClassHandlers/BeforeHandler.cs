using System;
using System.Threading.Tasks;

namespace MyNUnit.Runner.TestClassHandlers
{
    /// <summary>
    /// Represents handler in chain for handling method with before attribute.
    /// </summary>
    public class BeforeHandler : MyNUnitHandler
    {
        /// <summary>
        /// Initialize a new instance of the <see cref="BeforeHandler"/> class with specified next handlers.
        /// </summary>
        /// <param name="nextHandlerIfHandleSuccess"></param>
        /// <param name="nextHandlerIfHandleFailed"></param>
        public BeforeHandler(MyNUnitHandler? nextHandlerIfHandleSuccess = null,
            MyNUnitHandler? nextHandlerIfHandleFailed = null)
            : base(nextHandlerIfHandleSuccess, nextHandlerIfHandleFailed)
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