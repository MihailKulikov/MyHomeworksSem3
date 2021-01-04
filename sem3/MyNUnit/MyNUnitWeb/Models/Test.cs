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
    
    public class TestDb
    {
        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; }
        [EnumDataType(typeof(TestStatus))]
        public TestStatus Status { get; set; }
        public TimeSpan ElapsedTime { get; set; }
        public string? ReasonForIgnoring { get; set; }
        public Guid AssemblyId { get; set; }

        [ForeignKey("AssemblyId")]
        public virtual AssemblyDb Assembly { get; set; }
    }
}