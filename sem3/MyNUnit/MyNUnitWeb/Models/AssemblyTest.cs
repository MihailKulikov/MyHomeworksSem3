using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyNUnitWeb.Models
{
    public class AssemblyTest
    {
        public Guid AssemblyId { get; set; }
        public Assembly Assembly { get; set; }
        public Guid TestId { get; set; }
        public Test Test { get; set; }
    }

    public class AssemblyTestConfiguration : IEntityTypeConfiguration<AssemblyTest>
    {
        public void Configure(EntityTypeBuilder<AssemblyTest> builder)
        {
            builder.HasKey(assemblyTest => new { assemblyTest.AssemblyId, assemblyTest.TestId });
            builder.HasOne(assemblyTest => assemblyTest.Assembly)
                .WithMany(assembly => assembly.Tests)
                .HasForeignKey(assemblyTest => assemblyTest.AssemblyId);
        }
    }
}
