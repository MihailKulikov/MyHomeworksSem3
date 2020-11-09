namespace MyNUnit.Runner.Interfaces
{
    /// <summary>
    /// Represents test method in MyNUnit.
    /// </summary>
    public interface ITestMethod
    {
        /// <summary>
        /// Gets name of the test method.
        /// </summary>
        string Name { get; }
    }
}