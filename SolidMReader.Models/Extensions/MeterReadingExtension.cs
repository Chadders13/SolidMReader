using SolidMReader.Models.DTO;
using SolidMReader.Services.Interfaces;

namespace SolidMReader.Models.Extensions;

public static class MeterReadingExtension
{
    public static ValidMeterReading ValidateReadings(this List<MeterReading> readings, IValidation<MeterReading> validator)
    {
        ValidMeterReading output = new();
        
        foreach (var reading in readings)
        {
            if (validator.IsValid(reading) 
                && !output.ValidReadings.Any(x => x.AccountId == reading.AccountId && x.MeterReadValue == reading.MeterReadValue))
            {
                output.ValidReadings.Add(reading);
            }
            else
            {
                output.FailedReadings.Add(reading);
            }
        }

        return output;
    } 
}