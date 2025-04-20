using meter_reading_sharedKernal.Entities;
using Microsoft.EntityFrameworkCore;

namespace meter_readings_infrastructure.Entities;

public class ReadingsDbContext(DbContextOptions<ReadingsDbContext> options) : DbContext(options)
{
    public DbSet<MeterReadingDbRecord> MeterReadings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MeterReadingDbRecord>().ToTable("MeterReadings");
        modelBuilder.Entity<MeterReadingDbRecord>().HasKey(a => a.Id);
    }
}