using meter_reading_sharedKernal.Entities;
using meter_readings_application.Entities;

namespace meter_readings_application.Interfaces
{
    public interface IReadingValidationService
    {
        Task<IsRecordValid> IsRecordValid(MeterReading record);
    }
}