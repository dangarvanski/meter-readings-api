using AutoFixture.NUnit3;
using meter_reading_sharedKernal.Entities;
using meter_readings_application.Queries;
using meter_readings_infrastructure.Interfaces;
using Moq;
using NUnit.Framework;

namespace meter_readings_tests_unit.Queries;

[TestFixture]
[Parallelizable]
public class GetRecordsByAccountIdQueryTests
{
    private Mock<IMeterReadingRepository> _recordRepositoryMock;
    private Mock<IAccountRepository> _accountRepositoryMock;

    [SetUp]
    public void SetUp()
    {
        _recordRepositoryMock = new Mock<IMeterReadingRepository>();
        _accountRepositoryMock = new Mock<IAccountRepository>();
    }

    [Test]
    [AutoData]
    public async Task WhenAValidAccountIdIsPassed_ReturnsAllRecordsForAccount(List<MeterReadingDbRecord> testRecords)
    {
        // Arange
        int accountId = testRecords.First().Id;
        var response = testRecords.Where(x => x.Id == accountId).ToList();

        _accountRepositoryMock.Setup(r => r.AccountExistsAsync(It.IsAny<int>()))
            .ReturnsAsync(true);

        _recordRepositoryMock.Setup(r => r.GetRecordsByAccountIdAsync(accountId))
            .ReturnsAsync(response);

        var query = new GetRecordsByAccountIdQuery(accountId);
        var queryHandler = new GetRecordsByAccountIdQueryHandler(_accountRepositoryMock.Object, _recordRepositoryMock.Object);

        // Act
        var result = await queryHandler.Handle(query, It.IsAny<CancellationToken>());

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result!.Success, Is.True);
            Assert.That(result.Message, Is.EqualTo(string.Empty));
            Assert.That(result.MeterReadings, Is.EqualTo(response));
        });
    }

    [Test]
    public async Task WhenAValidAccountIdIsPassed_AndNoRecordsFound_ReturnsMessageWithEmptyList()
    {
        // Arrange  
        int accountId = 87;

        _accountRepositoryMock.Setup(r => r.AccountExistsAsync(It.IsAny<int>()))
            .ReturnsAsync(true);

        _recordRepositoryMock.Setup(r => r.GetRecordsByAccountIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new List<MeterReadingDbRecord>());

        var query = new GetRecordsByAccountIdQuery(accountId);
        var queryHandler = new GetRecordsByAccountIdQueryHandler(_accountRepositoryMock.Object, _recordRepositoryMock.Object);
        
        // Act
        var result = await queryHandler.Handle(query, It.IsAny<CancellationToken>());

        // Assert  
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result!.Success, Is.False);
            Assert.That(result.Message, Is.EqualTo($"No records were found for AccountId: {accountId}"));
            Assert.That(result.MeterReadings, Is.Null);
        });
    }

    [Test]
    public async Task WhenAValidAccountIdIsPassed_AndAccountDoesNotExist_ReturnsMessageWithEmptyList()
    {
        // Arrange 
        int accountId = 87;

        _accountRepositoryMock.Setup(r => r.AccountExistsAsync(It.IsAny<int>()))
            .ReturnsAsync(false);

        var query = new GetRecordsByAccountIdQuery(accountId);
        var queryHandler = new GetRecordsByAccountIdQueryHandler(_accountRepositoryMock.Object, _recordRepositoryMock.Object);

        // Act
        var result = await queryHandler.Handle(query, It.IsAny<CancellationToken>());

        // Assert  
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result!.Success, Is.False);
            Assert.That(result.Message, Is.EqualTo($"Account with ID: {accountId} was not found!"));
            Assert.That(result.MeterReadings, Is.Null);
        });
    }
}
