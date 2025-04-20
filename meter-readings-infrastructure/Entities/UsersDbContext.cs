using Microsoft.EntityFrameworkCore;

namespace meter_readings_infrastructure.Entities;

public class UsersDbContext(DbContextOptions<UsersDbContext> options) : DbContext(options)
{
    public DbSet<AccountDbRecord> TestAccounts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccountDbRecord>().ToTable("TestAccounts");
        modelBuilder.Entity<AccountDbRecord>().HasKey(a => a.AccountId);
    }
}
