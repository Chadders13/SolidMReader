using SolidMReader.Models.DTO;

namespace SolidMReader.Services.Interfaces;

public interface IMeterReadingsRepository
{
    public Task<MeterReading> GetMeterReading(Guid meterReadingGuid);

    bool IsDuplicateForAccount(MeterReading reading);
    Task<MeterReading?> AddValidReadings(List<MeterReading> validReadings);
    Task<MeterReading?> GetAccountLastMeterReading(int accountId);
}