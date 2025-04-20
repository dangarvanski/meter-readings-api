using MediatR;
using meter_reading_sharedKernal.Entities;
using meter_readings_application.Interfaces;
using meter_readings_infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace meter_readings_application.Commands;

public record UploadCsvCommand(IFormFile File) : IRequest<UploadCsvResult>;

public sealed class UploadCsvCommandHandler : IRequestHandler<UploadCsvCommand, UploadCsvResult>
{
    private readonly ILogger<UploadCsvCommandHandler> _logger;
    private readonly IMeterReadingRepository _meterReadingRepository;
    private readonly ICsvProcessingService _csvProcessingService;
    private readonly IReadingRecordValidationService _readingValidationService;

    public UploadCsvCommandHandler(
        ILogger<UploadCsvCommandHandler> logger,
        IMeterReadingRepository meterReadingRepository,
        ICsvProcessingService csvProcessingService,
        IReadingRecordValidationService readingValidationService)
    {
        _logger = logger;
        _meterReadingRepository = meterReadingRepository;
        _csvProcessingService = csvProcessingService;
        _readingValidationService = readingValidationService;
    }

    public async Task<UploadCsvResult> Handle(UploadCsvCommand request, CancellationToken cancellationToken)
    {
        var processCsvResponse = await _csvProcessingService.ProcessCsv(request.File, cancellationToken);

        if (!processCsvResponse.IsSuccessful)
        {
            return UploadCsvResult.Failure(processCsvResponse.Message);
        }

        var fileReadings = processCsvResponse.Records;

        // Validate records
        int goodRecords = 0, badRecords = 0;
        var validReadings = new List<MeterReadingDbRecord>();

        foreach (var reading in fileReadings)
        {
            var recordValidResult = await _readingValidationService.IsRecordValid(reading);

            if (recordValidResult.IsValid == false)
            {
                _logger.LogWarning(recordValidResult.Message);
                badRecords++;
            }
            else
            {
                validReadings.Add(reading);
                goodRecords++;
            }
        }

        if (!validReadings.Any())
        {
            _logger.LogWarning($"CSV file has already been uploaded or is empty: {request.File.FileName}");
            return UploadCsvResult.Failure("CSV file has been uploaded or contains no records.");
        }

        // Save valid readings
        await _meterReadingRepository.UploadMeterReadingsAsync(validReadings);
        _logger.LogInformation($"Successfully processed CSV file {request.File.FileName} with {fileReadings.Count} records.");

        return UploadCsvResult.Success(
            message: "CSV file processed successfully.",
            successRecordCount: goodRecords,
            failedRecordCount: badRecords
        );
    }
}
