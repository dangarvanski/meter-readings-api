using meter_reading_sharedKernal.Entities;

namespace meter_readings_infrastructure.Interfaces;

public interface IMeterReadingRepository
{
    Task<List<MeterReadingDbRecord>> GetAllRecordsAsync(int page, int pageSize);
    Task<List<MeterReadingDbRecord>> GetRecordsByAccountIdAsync(int accountId);
    Task UploadMeterReadingsAsync(IEnumerable<MeterReadingDbRecord> readings);
    Task<bool> CheckMeterReadingExists(MeterReadingDbRecord reading);
}