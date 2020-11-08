using System;

namespace MyNUnit.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class TestAttribute : Attribute
    {
        private Type? expected;

        public Type? Expected
        {
            get => expected;
            set
            {
                if (value == null || value.IsSubclassOf(typeof(Exception)))
                {
                    expected = value;
                }
                else
                {
                    throw new ArgumentException(nameof(value));
                }
            }
        }

        public string? Ignore { get; set; }
    }

    public class tesingAttributes
    {
        [Test(Expected = typeof(object))]
        public void firstMethod()
        {
            
        }

        [Test()]
        public void secondMethod()
        {
            
        }
    }
}
