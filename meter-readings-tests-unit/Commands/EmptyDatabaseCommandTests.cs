using meter_readings_application.Commands;
using meter_readings_infrastructure.Interfaces;
using Moq;
using NUnit.Framework;

namespace meter_readings_tests_unit.Commands;

[TestFixture]
[Parallelizable]
public class EmptyDatabaseCommandTests
{
    [Test]
    public async Task WhenEmptingDatabase_Success_ReturnsTrue()
    {
        // Arrange
        string confirmationString = "Delete";
        var repository = new Mock<IMeterReadingRepository>();

        repository.Setup(r => r.EmptyDatabaseAsync())
            .ReturnsAsync(true);

        var query = new EmptyDatabaseCommand(confirmationString);
        var queryHandler = new EmptyDatabaseCommandHandler(repository.Object);

        // Act  
        var result = await queryHandler.Handle(query, It.IsAny<CancellationToken>());

        // Assert  
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task WhenEmptingDatabase_Failure_ReturnsFalse()
    {
        // Arrange
        string confirmationString = "Delete";
        var repository = new Mock<IMeterReadingRepository>();

        repository.Setup(r => r.EmptyDatabaseAsync())
            .ReturnsAsync(false);

        var query = new EmptyDatabaseCommand(confirmationString);
        var queryHandler = new EmptyDatabaseCommandHandler(repository.Object);

        // Act  
        var result = await queryHandler.Handle(query, It.IsAny<CancellationToken>());

        // Assert  
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task WhenEmptingDatabase_WithIncorrectConfirmationString_ReturnsFalse()
    {
        // Arrange
        string confirmationString = "Delete";
        var repository = new Mock<IMeterReadingRepository>();

        var query = new EmptyDatabaseCommand(confirmationString);
        var queryHandler = new EmptyDatabaseCommandHandler(repository.Object);

        // Act  
        var result = await queryHandler.Handle(query, It.IsAny<CancellationToken>());

        // Assert  
        Assert.That(result, Is.False);
    }
}
