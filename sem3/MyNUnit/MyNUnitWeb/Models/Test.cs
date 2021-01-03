using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyNUnitWeb.Models
{
    public enum TestStatus
    {
        Success, 
        Failed, 
        Ignored
    }
    
    public class Test
    {
        [Key]
        public Guid Id { get; set; }

        [EnumDataType(typeof(TestStatus))]
        public TestStatus Status { get; set; }
        public TimeSpan ElapsedTime { get; set; }
        public string? ReasonForIgnoring { get; set; }
        public Guid AssemblyId { get; set; }

        [ForeignKey("AssemblyId")]
        public virtual Assembly Assembly { get; set; }
    }
}