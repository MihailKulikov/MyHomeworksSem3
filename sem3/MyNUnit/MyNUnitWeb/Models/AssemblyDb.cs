using System.Collections.Generic;

namespace MyNUnitWeb.Models
{
    public class Assembly
    {
        public int AssemblyId { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Test> Tests { get; set; }
    }
}