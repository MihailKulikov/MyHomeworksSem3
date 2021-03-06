﻿using System;

namespace MyNUnit.Attributes
{
    /// <summary>
    /// Identifies a method to be called immediately before each test is run.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class BeforeAttribute : Attribute
    {
    }
}