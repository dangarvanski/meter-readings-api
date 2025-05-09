﻿using meter_reading_sharedKernal.Entities;

namespace meter_readings_infrastructure.Interfaces;

public interface IMeterReadingRepository
{
    Task<List<MeterReadingDbRecord>> GetAllRecordsAsync(int page, int pageSize);
    Task<List<MeterReadingDbRecord>> GetRecordsByAccountIdAsync(int accountId);
    Task<bool> UploadMeterReadingsAsync(IEnumerable<MeterReadingDbRecord> readings);
    Task<bool> CheckMeterReadingExists(MeterReadingDbRecord reading);
    Task<MeterReadingDbRecord?> GetLastReadingForAccount(int accountId);
    Task<bool> EmptyDatabaseAsync();
}