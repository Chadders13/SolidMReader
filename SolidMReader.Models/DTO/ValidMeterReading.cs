namespace SolidMReader.Models.DTO;

public class ValidMeterReading
{
    public List<MeterReading> ValidReadings { get; set; } = new();
    public List<MeterReading> FailedReadings { get; set; } = new();
}