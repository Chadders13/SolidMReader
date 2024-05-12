using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SolidMReader.Data.Context;
using MeterReadingDTO = SolidMReader.Models.DTO.MeterReading;
using MeterReadingEntity = SolidMReader.Data.Entities.MeterReading;
using SolidMReader.Services.Interfaces;

namespace SolidMReader.Services.Repositories;

public class MeterReadingsRepository : IMeterReadingsRepository
{
    private readonly SolidMReaderContext _dbContext;
    private readonly ILogger<MeterReadingsRepository> _logger;

    public MeterReadingsRepository(SolidMReaderContext dbContext, ILogger<MeterReadingsRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
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
        bool output =  _dbContext.MeterReadings.Any(x =>
            x.AccountId == reading.AccountId && x.MeterReadValue == reading.MeterReadValue);
        return output;
    }

    public async Task<bool> AddValidReadings(List<MeterReadingDTO> validReadings)
    {
        try
        {
            foreach (var meterReading in validReadings)
            {
                MeterReadingEntity reading = new ()
                {
                    AccountId = meterReading.AccountId,
                    MeterReadingGuid = Guid.NewGuid(),
                    MeterReadValue = meterReading.MeterReadValue,
                    MeterReadingDateTime = meterReading.MeterReadingDateTime
                };
                _dbContext.MeterReadings.Add(reading);
            }
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to Add meter readings");
            return false;
        }

        return true;
    }

    public async Task<MeterReadingDTO?> GetAccountLastMeterReading(int accountId)
    {
        var lastReading = await _dbContext.MeterReadings
            .Where(x => x.AccountId == accountId)
            .OrderByDescending(x => x.MeterReadingDateTime)
            .FirstOrDefaultAsync();

        if (lastReading == null)
        {
            return null;
        }
        
        MeterReadingDTO result = new(meterReadingGuid: lastReading.MeterReadingGuid, accountId: lastReading.AccountId,
            meterReadValue: lastReading.MeterReadValue, meterReadingDateTime: lastReading.MeterReadingDateTime);

        return result;
    }

    public bool IsLowerThanCurrentReading(MeterReadingDTO reading)
    {
        return _dbContext.MeterReadings
            .Any(x => x.AccountId == reading.AccountId && 
                      x.MeterReadingDateTime < reading.MeterReadingDateTime &&
                      x.MeterReadValue > reading.MeterReadValue);
    }
}