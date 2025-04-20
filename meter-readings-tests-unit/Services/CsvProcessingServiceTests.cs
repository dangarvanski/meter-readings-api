using meter_readings_application.Commands;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Text;
using AutoFixture.NUnit3;
using meter_reading_sharedKernal.Entities;
using meter_readings_application.Services;

namespace meter_readings_tests_unit.Services;

[TestFixture]
[Parallelizable]
public class CsvProcessingServiceTests
{
    private Mock<ILogger<UploadCsvCommandHandler>> _loggerMock;
    private CsvProcessingService _service;

    [SetUp]
    public void SetUp()
    {
        _loggerMock = new Mock<ILogger<UploadCsvCommandHandler>>();
        _service = new CsvProcessingService(_loggerMock.Object);
    }

    [Test]
    [AutoData]
    public async Task WhenAValidFileIsPassed_ReturnsASuccessObject(List<MeterReadingDbRecord> records)
    {
        // Arrange
        var csvContent = "AccountId,MeterReadingDateTime,MeterReadValue\n" +
                         "2233,24/05/2019 09:24,3455\n" +
                         "2233,25/05/2019 09:24,6248\n" +
                         "2233,26/05/2019 09:24,5267";

        var fileName = "test.csv";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

        FormFile testFormFile = new FormFile(stream, 0, stream.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = "text/csv"
        };

        // Act
        var result = await _service.ProcessCsv(testFormFile, It.IsAny<CancellationToken>());

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result!.IsSuccessful, Is.True);
            Assert.That(result.Message, Is.EqualTo(string.Empty));
            Assert.That(result.Records.Count, Is.AtLeast(3));
        });
    }

    [Test]
    [AutoData]
    public async Task WhenInvalidFileIsPassed_ReturnsAFailureObject(List<MeterReadingDbRecord> records)
    {
        // Arrange
        var csvContent = "I'm not a valid file!";

        var fileName = "test.csv";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

        FormFile testFormFile = new FormFile(stream, 0, stream.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = "text/csv"
        };

        // Act
        var result = await _service.ProcessCsv(testFormFile, It.IsAny<CancellationToken>());

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result!.IsSuccessful, Is.False);
            Assert.That(result.Message, Is.EqualTo("Failed to process CSV file."));
            Assert.That(result.Records.Count, Is.EqualTo(0));
        });
    }
}
