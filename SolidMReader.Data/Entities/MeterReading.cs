namespace SolidMReader.Data.Entities;

public partial class MeterReading
{
    public Guid MeterReadingGuid { get; set; }

    public int AccountId { get; set; }

    public DateTime MeterReadingDateTime { get; set; }

    public int MeterReadValue { get; set; }

    public virtual Account Account { get; set; } = null!;
}
