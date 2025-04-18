using meter_reading_sharedKernal.Entities;
using meter_readings_application.Entities;
using meter_readings_application.Interfaces;
using meter_readings_infrastructure.Interfaces;

namespace meter_readings_application;

public class ReadingValidationService : IReadingValidationService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IMeterReadingRepository _meterReadingRepository;

    public ReadingValidationService(IAccountRepository accountRepository, IMeterReadingRepository meterReadingRepository)
    {
        _accountRepository = accountRepository;
        _meterReadingRepository = meterReadingRepository;
    }

    public async Task<IsRecordValid> IsRecordValid(MeterReading record)
    {
        var response = new IsRecordValid
        {
            IsValid = true,
            Message = string.Empty
        };

        if (await IsRecordDuplicate(record))
        {
            response.IsValid = false;
            response.Message = $"Record already registered in the database for AccountId: {record.AccountId}, ReadingValue: {record.ReadingValue}, Date: {record.ReadingDate.Date}";
            return response;
        }

        if (!await AccountForRecordExists(record))
        {
            response.IsValid = false;
            response.Message = $"No account has been found with AccountID: {record.AccountId}";
            return response;
        }

        return response;
    }

    private async Task<bool> AccountForRecordExists(MeterReading record)
    {
        if (!await _accountRepository.AccountExistsAsync(record.AccountId))
        {
            return false;
        }
        return true;
    }

    private async Task<bool> IsRecordDuplicate(MeterReading record)
    {
        // Check for duplicate record in the database.
        if (!await _meterReadingRepository.CheckMeterReadingExists(record))
        {
            return false;
        }
        return true;
    }
}
