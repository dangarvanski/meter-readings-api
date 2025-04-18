using meter_reading_sharedKernal.Entities;
using Microsoft.EntityFrameworkCore;

namespace meter_readings_infrastructure.Entities;

public class ReadingsDbContext : DbContext
{
    public ReadingsDbContext(DbContextOptions<ReadingsDbContext> options) : base(options) { }

    public DbSet<MeterReading> MeterReadings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MeterReading>().ToTable("MeterReadings");
        modelBuilder.Entity<MeterReading>().HasKey(a => a.Id);
        modelBuilder.Entity<MeterReading>().Property(m => m.AccountId).HasMaxLength(50);
    }
}