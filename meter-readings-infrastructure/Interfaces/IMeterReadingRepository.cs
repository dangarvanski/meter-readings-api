using meter_reading_sharedKernal.Entities;

namespace meter_readings_infrastructure.Interfaces;

public interface IMeterReadingRepository
{
    Task UploadMeterReadingsAsync(IEnumerable<MeterReadingDbRecord> readings);
    Task<bool> CheckMeterReadingExists(MeterReadingDbRecord reading);
}