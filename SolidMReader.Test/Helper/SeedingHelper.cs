using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using SolidMReader.Data.Context;
using SolidMReader.Data.Entities;
using SolidMReader.Models.DTO;
using MeterReading = SolidMReader.Data.Entities.MeterReading;
using MeterReadingDto = SolidMReader.Models.DTO.MeterReading;

namespace SolidMReader.Test.Helper;

public static class SeedingHelper
{
    private static List<int> AccountIdsSeeded { get; set; } = new ();
    public static string? Token { get; private set; }

    public static async Task SetAuthorizationToken(this WebApplicationFactory<Program> env, HttpClient client)
    {
        UserLoginDto userLogin = new ()
        {
            Username = "darth",
            Password = "ga1ax4"
        };

        var json = JsonSerializer.Serialize(userLogin);
        
        StringContent stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

        var response = await client.PostAsync("/api/authenticate", stringContent);

        if (response.IsSuccessStatusCode)
        {
            var tokenString = await response.Content.ReadAsStringAsync();
            JsonSerializerOptions? jso = new()
            {
                PropertyNameCaseInsensitive = true
            };
            AuthToken? result = JsonSerializer.Deserialize<AuthToken>(tokenString, jso);
            Token = result?.Token;
            
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token}");
        }
        else
        {
            throw new Exception("Issue resolving token");
        }
    }

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

    class AuthToken
    {
        public string Token { get; set; }
    }
}