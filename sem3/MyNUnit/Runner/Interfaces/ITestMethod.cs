namespace MyNUnit.Runner.Interfaces
{
    /// <summary>
    /// Represents test method in MyNUnit.
    /// </summary>
    public interface ITestMethod
    {
        /// <summary>
        /// Gets name of the test.
        /// </summary>
        string Name { get; }
    }
}