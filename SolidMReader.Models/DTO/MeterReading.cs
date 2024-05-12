namespace SolidMReader.Models.DTO;

public class MeterReading
{
    public MeterReading(Guid meterReadingGuid, int accountId, int meterReadValue, DateTime meterReadingDateTime)
    {
        MeterReadingGuid = meterReadingGuid;
        AccountId = accountId;
        MeterReadValue = meterReadValue;
        MeterReadingDateTime = meterReadingDateTime;
    }

    public Guid MeterReadingGuid { get; set; }
    public int AccountId { get; set; }
    public DateTime MeterReadingDateTime { get; set; }
    
    public int MeterReadValue { get; set; }
}