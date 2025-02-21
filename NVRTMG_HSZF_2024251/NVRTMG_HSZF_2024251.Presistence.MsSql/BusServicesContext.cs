using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using NVRTMG_HSZF_2024251.Model;

namespace NVRTMG_HSZF_2024251.Presistence.MsSql;

public partial class BusServicesContext : DbContext
{
    public BusServicesContext()
    {
    }

    public BusServicesContext(DbContextOptions<BusServicesContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BusService> BusServices { get; set; }

    public virtual DbSet<Region> Regions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=(LocalDb)\\MSSQLLocalDB;Database=BusServices;Trusted_Connection=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BusService>(entity =>
        {
            entity.HasIndex(e => e.RegionId, "IX_BusServices_RegionId");

            entity.HasOne(d => d.Region).WithMany(p => p.BusServices).HasForeignKey(d => d.RegionId);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
