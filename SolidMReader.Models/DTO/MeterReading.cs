using CsvHelper.Configuration.Attributes;

namespace SolidMReader.Models.DTO;

public class MeterReading
{
    public MeterReading()
    {
        
    }
    
    public MeterReading(Guid meterReadingGuid, int accountId, int meterReadValue, DateTime meterReadingDateTime)
    {
        MeterReadingGuid = meterReadingGuid;
        AccountId = accountId;
        MeterReadValue = meterReadValue;
        MeterReadingDateTime = meterReadingDateTime;
    }
    [Ignore]
    public Guid MeterReadingGuid { get; set; }
    [Name("AccountId")]
    public int AccountId { get; set; }
    [Name("MeterReadingDateTime")]
    [CultureInfo("en-GB")]
    [Format("dd/MM/yyyy HH:mm")]
    public DateTime MeterReadingDateTime { get; set; }
    [Name("MeterReadValue")]
    public int MeterReadValue { get; set; }
}