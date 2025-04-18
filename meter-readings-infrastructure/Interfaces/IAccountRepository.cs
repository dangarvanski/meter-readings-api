namespace meter_readings_infrastructure.Interfaces;

public interface IAccountRepository
{
    Task<bool> AccountExistsAsync(int accountId);
}