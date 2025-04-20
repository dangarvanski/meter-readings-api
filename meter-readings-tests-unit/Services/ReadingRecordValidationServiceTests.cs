using AutoFixture.NUnit3;
using meter_reading_sharedKernal.Entities;
using meter_readings_application.Services;
using meter_readings_infrastructure.Interfaces;
using Moq;
using NUnit.Framework;

namespace meter_readings_tests_unit.Services;

[TestFixture]
[Parallelizable]
public class ReadingRecordValidationServiceTests
{
    private Mock<IAccountRepository> _accountRepository;
    private Mock<IMeterReadingRepository> _meterReadingRepository;
    private ReadingRecordValidationService _service;
    private MeterReadingDbRecord _testReading;

    [SetUp]
    public void SetUp()
    {
        _accountRepository = new Mock<IAccountRepository>();
        _meterReadingRepository = new Mock<IMeterReadingRepository>();
        _service = new ReadingRecordValidationService(_accountRepository.Object, _meterReadingRepository.Object);

        _testReading = new MeterReadingDbRecord
        {
            Id = 1,
            AccountId = 2233,
            ReadingDate = DateTime.UtcNow,
            ReadingValue = 12345
        };
    }

    [Test]
    [AutoData]
    public async Task WhenAValidReadingIsPassed_ReturnsASuccessObject(List<MeterReadingDbRecord> records)
    {
        // Arrange
        _accountRepository.Setup(r => r.AccountExistsAsync(It.IsAny<int>()))
            .ReturnsAsync(true);

        _meterReadingRepository.Setup(r => r.CheckMeterReadingExists(It.IsAny<MeterReadingDbRecord>()))
            .ReturnsAsync(false);

        _meterReadingRepository.Setup(r => r.GetLastReadingForAccount(It.IsAny<int>()));

        // Act
        var result = await _service.IsRecordValid(_testReading);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result!.IsValid, Is.True);
            Assert.That(result.Message, Is.EqualTo(string.Empty));
        });
    }

    [Test]
    [Ignore("Uncomment Regex check in ReadingRecordValidationService.IsValid() to run this test!")] // This will cause WhenAReadingIsPassed_WintNegativeReadingValue_ReturnsAFailureObject to fail as the Regex check won't pass negative values!
    [AutoData]
    public async Task WhenAReadingIsPassed_WintInvalidReadingValue_ReturnsAFailureObject(List<MeterReadingDbRecord> records)
    {
        // Arrange
        _testReading.ReadingValue = 87;

        // Act
        var result = await _service.IsRecordValid(_testReading);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result!.IsValid, Is.False);
            Assert.That(result.Message, Is.EqualTo($"Ivalid Reading Value. ReadingValue: {_testReading.ReadingValue} for AccountID: {_testReading.AccountId}"));
        });
    }

    [Test]
    [AutoData]
    public async Task WhenAReadingIsPassed_WintNegativeReadingValue_ReturnsAFailureObject(List<MeterReadingDbRecord> records)
    {
        // Arrange
        _testReading.ReadingValue = -12233;

        // Act
        var result = await _service.IsRecordValid(_testReading);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result!.IsValid, Is.False);
            Assert.That(result.Message, Is.EqualTo($"ReadingValue can't be below zero. ReadingValue: {_testReading.ReadingValue} for AccountID: {_testReading.AccountId}"));
        });
    }

    [Test]
    [AutoData]
    public async Task WhenAReadingIsPassed_WhenAccountDoesNotExist_ReturnsAFailureObject(List<MeterReadingDbRecord> records)
    {
        // Arrange
        _accountRepository.Setup(r => r.AccountExistsAsync(It.IsAny<int>()))
            .ReturnsAsync(false);

        // Act
        var result = await _service.IsRecordValid(_testReading);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result!.IsValid, Is.False);
            Assert.That(result.Message, Is.EqualTo($"No account has been found with AccountID: {_testReading.AccountId}"));
        });
    }

    [Test]
    [AutoData]
    public async Task WhenAReadingIsPassed_WhenReadingIsDuplicate_ReturnsAFailureObject(List<MeterReadingDbRecord> records)
    {
        // Arrange
        _accountRepository.Setup(r => r.AccountExistsAsync(It.IsAny<int>()))
            .ReturnsAsync(true);

        _meterReadingRepository.Setup(r => r.CheckMeterReadingExists(It.IsAny<MeterReadingDbRecord>()))
            .ReturnsAsync(true);

        // Act
        var result = await _service.IsRecordValid(_testReading);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result!.IsValid, Is.False);
            Assert.That(result.Message, Is.EqualTo($"Record already registered in the database for AccountId: {_testReading.AccountId}, ReadingValue: {_testReading.ReadingValue}, Date: {_testReading.ReadingDate.Date}"));
        });
    }

    [Test]
    [AutoData]
    public async Task WhenAReadingIsPassed_WhenReadingIsOld_ReturnsAFailureObject(List<MeterReadingDbRecord> records)
    {
        // Arrange  
        _accountRepository.Setup(r => r.AccountExistsAsync(It.IsAny<int>()))
            .ReturnsAsync(true);

        _meterReadingRepository.Setup(r => r.CheckMeterReadingExists(It.IsAny<MeterReadingDbRecord>()))
            .ReturnsAsync(false);

        _meterReadingRepository.Setup(r => r.GetLastReadingForAccount(It.IsAny<int>()))
            .ReturnsAsync(new MeterReadingDbRecord { ReadingDate = DateTime.UtcNow.AddDays(1) });

        // Act  
        var result = await _service.IsRecordValid(_testReading);

        // Assert  
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result!.IsValid, Is.False);
            Assert.That(result.Message, Is.EqualTo($"Record with reading of: {_testReading.ReadingValue} is older than latest reading for AccountID: {_testReading.AccountId}"));
        });
    }
}
