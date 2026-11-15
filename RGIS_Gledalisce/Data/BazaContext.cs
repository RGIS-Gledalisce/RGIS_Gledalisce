using Microsoft.EntityFrameworkCore;
using RGIS_Gledalisce.Models;

namespace RGIS_Gledalisce.Data
{
    public class BazaContext : DbContext
    {
        public BazaContext(DbContextOptions<BazaContext> options)
            : base(options)
        {
        }

        public DbSet<Uporabnik> Uporabniki { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Uporabnik>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}
