using AutoFixture.NUnit3;
using meter_reading_sharedKernal.Entities;
using meter_readings_application.Commands;
using meter_readings_application.Entities;
using meter_readings_application.Interfaces;
using meter_readings_infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Text;

namespace meter_readings_tests_unit.Commands;

[TestFixture]
[Parallelizable]
public class UploadCsvCommandTests
{
    private Mock<ILogger<UploadCsvCommandHandler>> _loggerMock;
    private Mock<IMeterReadingRepository> _meterReadingRepository;
    private Mock<ICsvProcessingService> _csvProcessingService;
    private Mock<IReadingRecordValidationService> _readingRecordValidationService;
    private FormFile _testFormFile;

    [SetUp]
    public void SetUp()
    {
        _loggerMock = new Mock<ILogger<UploadCsvCommandHandler>>();
        _meterReadingRepository = new Mock<IMeterReadingRepository>();
        _csvProcessingService = new Mock<ICsvProcessingService>();
        _readingRecordValidationService = new Mock<IReadingRecordValidationService>();

        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.FileName).Returns("test.csv");
        fileMock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(Encoding.UTF8.GetBytes("Sample CSV content")));
        fileMock.Setup(f => f.Length).Returns(Encoding.UTF8.GetBytes("Sample CSV content").Length);

        var fileMockObject = fileMock.Object;
        var content = "Sample CSV content";
        var fileName = "test.csv";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

        _testFormFile = new FormFile(stream, 0, stream.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = "text/csv"
        };
    }

    [Test]
    [AutoData]
    public async Task WhenAValidFileIsPassed_ReturnsASuccessObject(List<MeterReadingDbRecord> records)
    {
        // Arange
        _csvProcessingService.Setup(r => r.ProcessCsv(It.IsAny<IFormFile>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CsvProcessedResponse.Success(records));

        _readingRecordValidationService.Setup(r => r.IsRecordValid(It.IsAny<MeterReadingDbRecord>()))
            .ReturnsAsync(new RecordValidationResponse { IsValid = true, Message = string.Empty });

        _meterReadingRepository.Setup(r => r.UploadMeterReadingsAsync(It.IsAny<IEnumerable<MeterReadingDbRecord>>()))
            .ReturnsAsync(true);

        var query = new UploadCsvCommand(_testFormFile);
        var queryHandler = new UploadCsvCommandHandler(_loggerMock.Object, _meterReadingRepository.Object, _csvProcessingService.Object, _readingRecordValidationService.Object);

        // Act
        var result = await queryHandler.Handle(query, It.IsAny<CancellationToken>());

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result!.IsSuccess, Is.True);
            Assert.That(result.Message, Is.EqualTo("CSV file processed successfully."));
            Assert.That(result.SuccessRecordCount, Is.AtLeast(1));
            Assert.That(result.FailedRecordCount, Is.EqualTo(0));
        });
    }

    [Test]
    public async Task WhenInvalidFileIsPassed_ReturnsAFailureObject()
    {
        // Arange
        string failedMessage = "Failed to process CSV file.";

        _csvProcessingService.Setup(r => r.ProcessCsv(It.IsAny<IFormFile>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CsvProcessedResponse.Failure(failedMessage));

        var query = new UploadCsvCommand(It.IsAny<IFormFile>());
        var queryHandler = new UploadCsvCommandHandler(_loggerMock.Object, _meterReadingRepository.Object, _csvProcessingService.Object, _readingRecordValidationService.Object);

        // Act
        var result = await queryHandler.Handle(query, It.IsAny<CancellationToken>());

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result!.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo(failedMessage));
            Assert.That(result.SuccessRecordCount, Is.AtLeast(0));
            Assert.That(result.FailedRecordCount, Is.EqualTo(0));
        });
    }

    [Test]
    [AutoData]
    public async Task WhenAValidFileIsPassed_WithInvalidReadings_ReturnsAFailureObject(List<MeterReadingDbRecord> records)
    {
        // Arange
        _csvProcessingService.Setup(r => r.ProcessCsv(It.IsAny<IFormFile>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CsvProcessedResponse.Success(records));

        _readingRecordValidationService.Setup(r => r.IsRecordValid(It.IsAny<MeterReadingDbRecord>()))
            .ReturnsAsync(new RecordValidationResponse { IsValid = false, Message = It.IsAny<string>() });

        var query = new UploadCsvCommand(_testFormFile);
        var queryHandler = new UploadCsvCommandHandler(_loggerMock.Object, _meterReadingRepository.Object, _csvProcessingService.Object, _readingRecordValidationService.Object);

        // Act
        var result = await queryHandler.Handle(query, It.IsAny<CancellationToken>());

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result!.IsSuccess, Is.False);
            Assert.That(result.Message, Is.EqualTo("CSV file already has been uploaded or contains no records."));
            Assert.That(result.SuccessRecordCount, Is.AtLeast(0));
            Assert.That(result.FailedRecordCount, Is.EqualTo(0));
        });
    }
}
