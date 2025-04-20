using meter_reading_sharedKernal.Entities;
namespace meter_readings_application.Entities;

public class CsvProcessedResponse
{
    public bool IsSuccessful { get; set; }
    public string Message { get; set; }
    public List<MeterReadingDbRecord> Records { get; set; }

    private CsvProcessedResponse(bool isSuccessful, string message, List<MeterReadingDbRecord> records)
    {
        IsSuccessful = isSuccessful;
        Message = message;
        Records = records;
    }

    public static CsvProcessedResponse Success(List<MeterReadingDbRecord> records)
    {
        return new CsvProcessedResponse(true, string.Empty, records);
    }

    public static CsvProcessedResponse Failure(string message)
    {
        return new CsvProcessedResponse(false, message, new List<MeterReadingDbRecord>());
    }
}
