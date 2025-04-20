using CsvHelper;
using CsvHelper.Configuration;
using meter_reading_sharedKernal.Entities;
using meter_readings_application.Commands;
using meter_readings_application.Entities;
using meter_readings_application.Helpers;
using meter_readings_application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Text;

namespace meter_readings_application.Services;

public class CsvProcessingService : ICsvProcessingService
{
    private readonly ILogger<UploadCsvCommandHandler> _logger;
    private List<MeterReadingDbRecord> fileRecords;

    public CsvProcessingService(ILogger<UploadCsvCommandHandler> logger)
    {
        _logger = logger;
    }

    public async Task<CsvProcessedResponse> ProcessCsv(IFormFile file, CancellationToken cancellationToken)
    {
        try
        {
            fileRecords = new List<MeterReadingDbRecord>();

            using (var stream = new StreamReader(file.OpenReadStream(), Encoding.UTF8))
            using (var csv = new CsvReader(stream, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                Delimiter = ",",
                BadDataFound = context =>
                {
                    _logger.LogWarning($"Bad CSV data at field {context.Field}: {context.RawRecord}");
                }
            }))
            {
                csv.Context.RegisterClassMap<MeterReadingMapper>();

                await foreach (var record in csv.GetRecordsAsync<MeterReadingDbRecord>(cancellationToken))
                {
                    if (!IsRecordInFileDuplicate(record))
                    {
                        fileRecords.Add(record);
                    }
                }
            }

            return CsvProcessedResponse.Success(fileRecords);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error processing CSV file {file.FileName}: {ex.Message}");
            return CsvProcessedResponse.Failure("Failed to process CSV file.");
        }
    }

    private bool IsRecordInFileDuplicate(MeterReadingDbRecord newRecord)
    {
        // Check for duplicates based on AccountId, ReadingValue, and the same day in ReadingDate
        bool isDuplicate = fileRecords.Any(r =>
            r.AccountId == newRecord.AccountId &&
            r.ReadingValue == newRecord.ReadingValue &&
            r.ReadingDate.Date == newRecord.ReadingDate.Date);

        if (!isDuplicate)
        {
            return false;
            //records.Add(newRecord);
        }
        else
        {
            _logger.LogWarning("Duplicate record detected for AccountId: {AccountId}, ReadingValue: {ReadingValue}, Date: {Date}",
                newRecord.AccountId, newRecord.ReadingValue, newRecord.ReadingDate.Date);
            return true;
        }
    }
}
