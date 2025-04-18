using CsvHelper;
using CsvHelper.Configuration;
using MediatR;
using meter_reading_sharedKernal.Entities;
using meter_readings_application.Helpers;
using meter_readings_application.Interfaces;
using meter_readings_infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Text;

namespace meter_readings_application.Commands;

public record UploadCsvCommand(IFormFile File) : IRequest<UploadCsvResult>;

public sealed class UploadCsvCommandHandler : IRequestHandler<UploadCsvCommand, UploadCsvResult>
{
    private readonly ILogger<UploadCsvCommandHandler> _logger;
    private readonly IMeterReadingRepository _meterReadingRepository;
    private readonly IReadingValidationService _readingValidationService;
    private List<MeterReading> fileRecords;

    public UploadCsvCommandHandler(ILogger<UploadCsvCommandHandler> logger, IMeterReadingRepository meterReadingRepository, IReadingValidationService readingValidationService)
    {
        _logger = logger;
        _meterReadingRepository = meterReadingRepository;
        _readingValidationService = readingValidationService;
    }

    public async Task<UploadCsvResult> Handle(UploadCsvCommand request, CancellationToken cancellationToken)
    {
        try
        {
            int goodRecords = 0, badRecords = 0;
            fileRecords = new List<MeterReading>();

            // Read and parse CSV file using CsvHelper
            using (var stream = new StreamReader(request.File.OpenReadStream(), Encoding.UTF8))
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
                csv.Context.RegisterClassMap<MeterReadingMap>();

                await foreach (var record in csv.GetRecordsAsync<MeterReading>(cancellationToken))
                {
                    if (!IsRecordInFileDuplicate(record))
                    {
                        fileRecords.Add(record);
                    }
                }
            }

            // Validate records
            var validRecords = new List<MeterReading>();

            foreach (var record in fileRecords)
            {
                var recordValidResult = await _readingValidationService.IsRecordValid(record);

                if (recordValidResult.IsValid == false)
                {
                    _logger.LogWarning(recordValidResult.Message);
                    badRecords++;
                }
                else
                {
                    validRecords.Add(record);
                    goodRecords++;
                }
            }

            if (!validRecords.Any())
            {
                _logger.LogWarning($"CSV file has already been uploaded or is empty: {request.File.FileName}");
                return UploadCsvResult.Failure("CSV file has been uploaded or contains no records.");
            }

            // Save valid readings
            await _meterReadingRepository.UploadMeterReadingsAsync(validRecords);
            _logger.LogInformation($"Successfully processed CSV file {request.File.FileName} with {fileRecords.Count} records.");

            return UploadCsvResult.Success(
                message: "CSV file processed successfully.",
                successRecordCount: goodRecords,
                failedRecordCount: badRecords
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error processing CSV file {request.File.FileName}: {ex.Message}");
            return UploadCsvResult.Failure("Failed to process CSV file.");
        }
    }

    private bool IsRecordInFileDuplicate(MeterReading newRecord)
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
