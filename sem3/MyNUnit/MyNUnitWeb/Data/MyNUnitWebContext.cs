using Microsoft.EntityFrameworkCore;
using MyNUnitWeb.Models;

namespace MyNUnitWeb.Data
{
    public class MyNUnitWebContext : DbContext
    {
        public MyNUnitWebContext (DbContextOptions<MyNUnitWebContext> options)
            : base(options)
        {
        }

        public DbSet<TestDb> Tests { get; set; }
        public DbSet<AssemblyDb> Assemblies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(System.Reflection.Assembly.GetExecutingAssembly());
        }
    }
}
