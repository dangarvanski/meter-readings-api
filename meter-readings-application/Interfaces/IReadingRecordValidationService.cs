using meter_reading_sharedKernal.Entities;
using meter_readings_application.Entities;

namespace meter_readings_application.Interfaces
{
    public interface IReadingRecordValidationService
    {
        Task<IsRecordValid> IsRecordValid(MeterReadingDbRecord record);
    }
}