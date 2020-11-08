using System;

namespace MyNUnit.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class AfterClassAttribute : Attribute
    { }
}