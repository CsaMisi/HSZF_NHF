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
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
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
