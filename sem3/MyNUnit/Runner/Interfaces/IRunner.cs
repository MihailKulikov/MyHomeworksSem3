using System.Collections.Generic;

namespace MyNUnit.Runner.Interfaces
{
    public interface IRunner
    {
        IEnumerable<TestResult> RunTests();
    }
}