using System;

namespace MyNUnit.Attributes
{
    /// <summary>
    /// Identifies a static method to be called once after all the child tests have run.
    /// The method is guaranteed to be called, even if an exception is thrown.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class BeforeClassAttribute : Attribute
    {
    }
}