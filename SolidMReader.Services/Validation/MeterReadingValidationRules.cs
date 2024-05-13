using System.Text.RegularExpressions;
using SolidMReader.Models.DTO;
using SolidMReader.Services.Interfaces;

namespace SolidMReader.Services.Validation;

public class MeterReadingValidationRules(
    IMeterReadingsRepository meterReadingsRepository,
    IAccountRepository accountRepository)
    : IValidation<MeterReading>
{
    public bool IsValid<T>(T reading) where T : MeterReading
    {
        return   IsValidAccountId(reading) &&
                 !IsDuplicateEntry(reading) &&
                 IsMeterReadingPosative(reading) &&
                 IsValidMeterReadValue(reading) &&
                 !IsNewReadingLowerThanCurrentReading(reading);
    }
    
    private bool IsDuplicateEntry(MeterReading reading)
    {
        return meterReadingsRepository.IsDuplicateForAccount(reading);
    }

    private bool IsValidAccountId(MeterReading reading)
    {
        return reading.AccountId > 0 && accountRepository.AccountExists(reading.AccountId);
    }
    
    private static bool IsMeterReadingPosative(MeterReading reading)
    {
        return reading.MeterReadValue > 0;
    }

    private bool IsValidMeterReadValue(MeterReading reading)
    {
        return Regex.IsMatch($"{reading.MeterReadValue:D5}", @"^0?[0-9]{5}$"); 
    }
    
    private bool IsNewReadingLowerThanCurrentReading(MeterReading reading)
    {
        bool output = meterReadingsRepository.IsLowerThanCurrentReading(reading);
        return output;
    }
}