namespace SolidMReader.Models.DTO;

public class ProcessCsvResult
{
    public List<MeterReading> ValidMeterReadings { get; set; }
    public List<string> FailedToParse { get; set; }
}