using AutoFixture.NUnit3;
using meter_reading_sharedKernal.Entities;
using meter_readings_application.Queries;
using meter_readings_infrastructure.Interfaces;
using Moq;
using NUnit.Framework;

namespace meter_readings_tests_unit.Queries;

[TestFixture]
[Parallelizable]
public class GetAllRecordsQueryTests
{
    [Test]
    [AutoData]
    public async Task WhenDataFound_ReturnsCorrectData(List<MeterReadingDbRecord> response)
    {
        // Arrange  
        var pageNumber = 1;
        var pageSize = 10;
        var repository = new Mock<IMeterReadingRepository>();

        repository.Setup(r => r.GetAllRecordsAsync(pageNumber, pageSize))
            .ReturnsAsync(response);

        var query = new GetAllRecordsQuery(pageNumber, pageSize);
        var queryHandler = new GetAllRecordsQueryHandler(repository.Object);

        // Act  
        var result = await queryHandler.Handle(query, It.IsAny<CancellationToken>());

        // Assert  
        Assert.That(result, Is.Not.Empty);
        Assert.That(result.Count, Is.AtMost(10));
    }

    [Test]
    public async Task WhenNoDataFound_ReturnsEmptyList()
    {
        // Arrange  
        var pageNumber = 1;
        var pageSize = 10;
        var repository = new Mock<IMeterReadingRepository>();

        repository.Setup(r => r.GetAllRecordsAsync(pageNumber, pageSize))
            .ReturnsAsync(new List<MeterReadingDbRecord>());

        var query = new GetAllRecordsQuery(pageNumber, pageSize);
        var queryHandler = new GetAllRecordsQueryHandler(repository.Object);

        // Act  
        var result = await queryHandler.Handle(query, It.IsAny<CancellationToken>());

        // Assert  
        Assert.That(result, Is.Empty);
    }
}
