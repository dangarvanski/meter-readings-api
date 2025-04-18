using Microsoft.EntityFrameworkCore;

namespace meter_readings_infrastructure.Entities;

public class UsersDbContext : DbContext
{
    public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options) { }
    public DbSet<Account> TestAccounts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>().ToTable("TestAccounts");
        modelBuilder.Entity<Account>().HasKey(a => a.AccountId);
    }
}

public class Account
{
    public int AccountId { get; set; }
}