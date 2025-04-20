using meter_reading_sharedKernal.Entities;

namespace meter_readings_application.Entities;

public class GetRecordsByAccountIdResponse
{
    public bool Success {  get; set; }

    public string Message { get; set; }

    public List<MeterReadingDbRecord> MeterReadings {  get; set; }
}
