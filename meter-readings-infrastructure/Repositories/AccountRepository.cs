using meter_readings_infrastructure.Entities;
using meter_readings_infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace meter_readings_infrastructure.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly UsersDbContext _context;

    public AccountRepository(UsersDbContext context)
    {
        _context = context;
    }

    public async Task<bool> AccountExistsAsync(int accountId)
    {
        return await _context.TestAccounts.AnyAsync(a => a.AccountId == accountId);
    }
}