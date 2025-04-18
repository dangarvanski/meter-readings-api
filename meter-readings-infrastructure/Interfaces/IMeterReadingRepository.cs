using meter_reading_sharedKernal.Entities;

namespace meter_readings_infrastructure.Interfaces;

public interface IMeterReadingRepository
{
    Task UploadMeterReadingsAsync(IEnumerable<MeterReading> readings);
    Task<bool> CheckMeterReadingExists(MeterReading reading);
}