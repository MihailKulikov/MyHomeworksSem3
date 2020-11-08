using System;

namespace MyNUnit.Attributes
{
    /// <summary>
    /// Identifies a method to be called immediately after each test is run.
    /// The method is guaranteed to be called, even if an exception is thrown.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class BeforeAttribute : Attribute
    {
        
    }
}