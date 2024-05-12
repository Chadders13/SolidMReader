using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using SolidMReader.Data.Context;
using SolidMReader.Data.Entities;
using MeterReadingDto = SolidMReader.Models.DTO.MeterReading;

namespace SolidMReader.Test.Helper;

public static class SeedingHelper
{
    private static List<int> AccountIdsSeeded { get; set; } = new ();
    public static void SeedAccounts(this WebApplicationFactory<Program> env, List<int> accountIds)
    {
        accountIds = accountIds.Distinct().ToList();
        
        using (var scope = env.Services.CreateScope())
        {
            SolidMReaderContext dbContext = scope.ServiceProvider.GetRequiredService<SolidMReaderContext>();
            foreach (var accountId in accountIds)
            {
                if (AccountIdsSeeded.Any(x => x == accountId))
                {
                    continue;    
                }
                
                Account a = new()
                {
                    AccountId = accountId,
                    FirstName = "Han",
                    LastName = "Solo"
                };

                dbContext.Accounts.Add(a);
                AccountIdsSeeded.Add(accountId);
            }

            dbContext.SaveChanges();
        }
    }

    public static void SeedLowerReadings(this WebApplicationFactory<Program> env, List<MeterReadingDto> csvNewReadings)
    {
        using (var scope = env.Services.CreateScope())
        {
            SolidMReaderContext dbContext = scope.ServiceProvider.GetRequiredService<SolidMReaderContext>();
            foreach (var reading in csvNewReadings)
            {
                MeterReading mr = new()
                {
                    AccountId = reading.AccountId,
                    MeterReadValue = reading.MeterReadValue + DateTime.UtcNow.Second,
                    MeterReadingGuid = Guid.NewGuid(),
                    MeterReadingDateTime = reading.MeterReadingDateTime.AddDays(-10)
                };

                dbContext.MeterReadings.Add(mr);
            }

            dbContext.SaveChanges();
        }
    }
}