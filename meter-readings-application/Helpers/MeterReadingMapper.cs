using CsvHelper.Configuration;
using meter_reading_sharedKernal.Entities;

namespace meter_readings_application.Helpers;

public class MeterReadingMapper : ClassMap<MeterReadingDbRecord>
{
    public MeterReadingMapper()
    {
        Map(m => m.AccountId)
            .Name("AccountId");
        Map(m => m.ReadingDate)
            .Name("MeterReadingDateTime")
            .TypeConverterOption
            .Format("dd/MM/yyyy HH:mm", "dd/MM/yy HH:mm", "dd/MM/yy H:mm");
        Map(m => m.ReadingValue)
            .Name("MeterReadValue");
    }
}