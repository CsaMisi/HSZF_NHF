using Microsoft.EntityFrameworkCore;
using NVRTMG_HSZF_2024251.Model;

namespace NVRTMG_HSZF_2024251.Presistence.MsSql
{
    public class AppDbContext : DbContext
    {
        public DbSet<Region> Regions { get; set; }
        public DbSet<BusService> BusServices { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Region>()
                .HasMany(r => r.BusServices)
                .WithOne(bs => bs.Region)
                .HasForeignKey(bs => bs.RegionId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
