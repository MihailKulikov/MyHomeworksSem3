using System.Collections.Generic;

namespace MyNUnit.Runner.Interfaces
{
    public interface IAssemblyHandler
    {
        IEnumerable<TestClassWrapper> TestClasses { get; }
    }
}