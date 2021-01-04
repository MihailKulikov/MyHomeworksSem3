using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyNUnitWeb.Models
{
    public class Assembly
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Test> Tests { get; set; }
    }
}