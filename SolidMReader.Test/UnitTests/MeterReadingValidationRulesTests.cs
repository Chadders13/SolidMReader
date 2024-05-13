using NSubstitute;
using SolidMReader.Models.DTO;
using SolidMReader.Services.Interfaces;
using SolidMReader.Services.Validation;

namespace SolidMReader.Test.UnitTests;

public class MeterReadingValidationRulesTests
{
    private readonly IMeterReadingsRepository _meterReadingsRepository = Substitute.For<IMeterReadingsRepository>();
    private readonly IAccountRepository _accountRepository = Substitute.For<IAccountRepository>();

    private MeterReadingValidationRules CreateRules() =>
        new (_meterReadingsRepository, _accountRepository);

    [Fact]
    public void IsValid_ValidReading_ReturnsTrue()
    {
        // Arrange
        var reading = new MeterReading { AccountId = 123, MeterReadValue = 12345 };
        _meterReadingsRepository.IsDuplicateForAccount(reading).Returns(false);
        _meterReadingsRepository.IsLowerThanCurrentReading(reading).Returns(false);
        _accountRepository.AccountExists(reading.AccountId).Returns(true);

        var rules = CreateRules();

        // Act
        var result = rules.IsValid(reading);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsValid_InvalidAccountId_ReturnsFalse()
    {
        // Arrange
        var reading = new MeterReading { AccountId = -1, MeterReadValue = 12345 };
        _accountRepository.AccountExists(reading.AccountId).Returns(false);
        var rules = CreateRules();

        // Act
        var result = rules.IsValid(reading);

        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData(111111, false)]
    [InlineData(11111, true)]
    [InlineData(12345, true)]
    [InlineData(-9999, false)]
    [InlineData(-1, false)]
    [InlineData(-999, false)]
    [InlineData(777, true)]
    public void IsValue_ValidMeterReadValue(int value, bool expectedOutcome)
    {
        // Arrange
        var reading = new MeterReading { AccountId = 42, MeterReadValue = value };
        
        _accountRepository.AccountExists(reading.AccountId).Returns(true);
        _meterReadingsRepository.IsDuplicateForAccount(reading).Returns(false);
        _meterReadingsRepository.IsLowerThanCurrentReading(reading).Returns(false);
        
        var rules = CreateRules();

        // Act
        var result = rules.IsValid(reading);

        // Assert
        Assert.Equal(expectedOutcome, result);
    }
    [Fact]
    public void IsNewReadingLowerThanCurrentReading_NoExistingReading_ReturnsFalse()
    {
        var reading = new MeterReading { AccountId = 123, MeterReadValue = 100 };

        _meterReadingsRepository
            .GetAccountLastMeterReading(reading.AccountId)
            .Returns((MeterReading)null);

        _accountRepository.AccountExists(reading.AccountId).Returns(true);
        _meterReadingsRepository.IsDuplicateForAccount(reading).Returns(false);
        
        var rules = CreateRules();

        var result = rules.IsValid(reading);

        Assert.True(result);
    }
    
    [Fact]
    public void IsNewReadingLowerThanCurrentReading_LowerValue_ReturnsTrue()
    {
        var reading = new MeterReading { AccountId = 123, MeterReadValue = 100 };

        _meterReadingsRepository
            .IsLowerThanCurrentReading(reading)
            .Returns(true);

        _accountRepository.AccountExists(reading.AccountId).Returns(true);
        _meterReadingsRepository.IsDuplicateForAccount(reading).Returns(false);
        
        var rules = CreateRules();

        var result = rules.IsValid(reading);

        Assert.False(result);
    }
    

}