using meter_reading_sharedKernal.Entities;
using meter_readings_application.Entities;
using meter_readings_application.Interfaces;
using meter_readings_infrastructure.Interfaces;
using System.Text.RegularExpressions;

namespace meter_readings_application.Services;

public class ReadingRecordValidationService : IReadingRecordValidationService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IMeterReadingRepository _meterReadingRepository;

    public ReadingRecordValidationService(IAccountRepository accountRepository, IMeterReadingRepository meterReadingRepository)
    {
        _accountRepository = accountRepository;
        _meterReadingRepository = meterReadingRepository;
    }

    public async Task<RecordValidationResponse> IsRecordValid(MeterReadingDbRecord record)
    {
        var response = new RecordValidationResponse
        {
            IsValid = true,
            Message = string.Empty
        };

        //if (!Regex.IsMatch(record.ReadingValue.ToString(), @"^\d{5}$"))
        //{
        //    response.IsValid = false;
        //    response.Message = $"Ivalid Reading Value. ReadingValue: {record.ReadingValue} for AccountID: {record.AccountId}";
        //    return response;
        //}

        if (record.ReadingValue < 0)
        {
            response.IsValid = false;
            response.Message = $"ReadingValue can't be below zero. ReadingValue: {record.ReadingValue} for AccountID: {record.AccountId}";
            return response;
        }

        if (!await AccountForRecordExists(record))
        {
            response.IsValid = false;
            response.Message = $"No account has been found with AccountID: {record.AccountId}";
            return response;
        }

        if (await IsRecordDuplicate(record))
        {
            response.IsValid = false;
            response.Message = $"Record already registered in the database for AccountId: {record.AccountId}, ReadingValue: {record.ReadingValue}, Date: {record.ReadingDate.Date}";
            return response;
        }

        if (await IsRecordOlder(record))
        {
            response.IsValid = false;
            response.Message = $"Record with reading of: {record.ReadingValue} is older than latest reading for AccountID: {record.AccountId}";
            return response;
        }

        return response;
    }

    private async Task<bool> AccountForRecordExists(MeterReadingDbRecord record)
    {
        if (!await _accountRepository.AccountExistsAsync(record.AccountId))
        {
            return false;
        }
        return true;
    }

    private async Task<bool> IsRecordDuplicate(MeterReadingDbRecord record)
    {
        if (!await _meterReadingRepository.CheckMeterReadingExists(record))
        {
            return false;
        }
        return true;
    }

    private async Task<bool> IsRecordOlder(MeterReadingDbRecord record)
    {
        MeterReadingDbRecord? latestRecord = await _meterReadingRepository.GetLastReadingForAccount(record.AccountId);

        if (latestRecord != null)
        {
            if (latestRecord.ReadingDate > record.ReadingDate)
            {
                return true;
            }
        }
        return false;
    }
}
