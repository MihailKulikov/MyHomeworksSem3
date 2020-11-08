using System;
using NUnit.Framework;

namespace MyNUnit.Attributes
{
    /// <summary>
    /// Identifies a static method that is called once to perform setup before test class initialization.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class AfterClassAttribute : Attribute
    { }
}