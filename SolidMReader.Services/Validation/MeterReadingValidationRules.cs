using System.Text.RegularExpressions;
using SolidMReader.Models.DTO;
using SolidMReader.Services.Interfaces;

namespace SolidMReader.Services.Validation;

public class MeterReadingValidationRules : IValidation<MeterReading>
{
    private readonly IMeterReadingsRepository _meterReadingsRepository;

    public MeterReadingValidationRules(IMeterReadingsRepository meterReadingsRepository)
    {
        _meterReadingsRepository = meterReadingsRepository;
    }
    
    private bool IsDuplicateEntry(MeterReading reading)
    {
        return _meterReadingsRepository.IsDuplicateForAccount(reading);
    }

    private bool IsValidAccountId(MeterReading reading)
    {
        return reading.AccountId > 0;
    }
    
    private static bool IsMeterReadingPosative(MeterReading reading)
    {
        return reading.MeterReadValue > 0;
    }

    private bool IsValidMeterReadValue(MeterReading reading)
    {
        return Regex.IsMatch(reading.MeterReadValue.ToString(), @"^0?[0-9]{5}$"); 
    }

    public bool IsValid<T>(T reading) where T : MeterReading
    {
        return !IsDuplicateEntry(reading) &&
               IsValidAccountId(reading) &&
               IsMeterReadingPosative(reading) &&
               IsValidMeterReadValue(reading);
    }
}