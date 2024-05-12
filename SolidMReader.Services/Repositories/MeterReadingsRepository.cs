using Microsoft.EntityFrameworkCore;
using SolidMReader.Data.Context;
using MeterReadingDTO = SolidMReader.Models.DTO.MeterReading;
using SolidMReader.Services.Interfaces;

namespace SolidMReader.Services.Repositories;

public class MeterReadingsRepository : IMeterReadingsRepository
{
    private readonly SolidMReaderContext _dbContext;

    public MeterReadingsRepository(SolidMReaderContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<MeterReadingDTO?> GetMeterReading(Guid meterReadingGuid)
    {
        var readingsResult = await _dbContext.MeterReadings.Where(x => x.MeterReadingGuid == meterReadingGuid).ToListAsync();

        var firstResult = readingsResult.FirstOrDefault();
        
        MeterReadingDTO result = new(meterReadingGuid: firstResult.MeterReadingGuid, accountId: firstResult.AccountId,
            meterReadValue: firstResult.MeterReadValue, meterReadingDateTime: firstResult.MeterReadingDateTime);
        
        return result;
    }

    public bool IsDuplicateForAccount(MeterReadingDTO reading)
    {
        throw new NotImplementedException();
    }

    public async Task<MeterReadingDTO?> AddValidReadings(List<MeterReadingDTO> validReadings)
    {
        throw new NotImplementedException();
    }

    public async Task<MeterReadingDTO?> GetAccountLastMeterReading(int accountId)
    {
        var lastReading = await _dbContext.MeterReadings
            .Where(x => x.AccountId == accountId)
            .OrderByDescending(x => x.MeterReadingDateTime)
            .FirstOrDefaultAsync();
        
        MeterReadingDTO result = new(meterReadingGuid: lastReading.MeterReadingGuid, accountId: lastReading.AccountId,
            meterReadValue: lastReading.MeterReadValue, meterReadingDateTime: lastReading.MeterReadingDateTime);

        return result;
    }
}