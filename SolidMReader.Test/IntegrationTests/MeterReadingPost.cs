using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SolidMReader.Data.Context;
using SolidMReader.Data.Entities;
using SolidMReader.Models.ViewModels;
using SolidMReader.Test.Helper;
using MeterReadingDto = SolidMReader.Models.DTO.MeterReading;

namespace SolidMReader.Test.IntegrationTests;

public class MeterReadingPost : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;

    public MeterReadingPost(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d =>
                    d.ServiceType == typeof(DbContextOptions<SolidMReaderContext>)
                );

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }
                
                services.AddDbContext<SolidMReaderContext>(options => // Replace with your actual DbContext
                    options.UseInMemoryDatabase("TestDb"));  // Choose a unique name 
            });
        });
    }
    
    public void Dispose()
    {
        
    }
    
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
    
    [Theory]
    [InlineData("VaildMeterReadings_3_20240511.csv", 3)]
    [InlineData("VaildMeterReadings_5_20240511.csv", 5)]
    public async Task PostCsv_ShouldReturnOkAndSuccessfullyUploadX(string fileName, int successReturnCount)
    {
        // Arrange
        var client = _factory.CreateClient();
        
        var projectDirectory = Directory.GetCurrentDirectory(); 
        var testDataFolder = Path.Combine(projectDirectory, "TestCsvData"); 
        var csvFilePath = Path.Combine(testDataFolder, fileName); 

        var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(File.ReadAllBytes(csvFilePath));

        List<MeterReadingDto> meterReadings = new();
        
        using (FileStream fileStream = File.OpenRead(csvFilePath))
        {
            meterReadings = CsvReaderHelper.ParseCsv(fileStream);
        }
        
        _factory.SeedAccounts(meterReadings.Select(x => x.AccountId).ToList());

        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("text/csv");
        content.Add(fileContent, "file", fileName);

        // Act
        var response = await client.PostAsync("/meter-reading-uploads", content);

        // Assert
        response.EnsureSuccessStatusCode();

        var resultContent = await response.Content.ReadAsStringAsync();
        var jsonResult = JsonSerializer.Deserialize<PostMeterReadingResult>(resultContent, _jsonSerializerOptions);
        
        if (string.IsNullOrWhiteSpace(resultContent) || jsonResult == null)
        {
            Assert.Fail("Missing data");
        }
        
        Assert.Equal(successReturnCount, jsonResult.Successful);
    }
    
    [Theory]
    [InlineData("DuplicateMeterReadings_20240511.csv", 6, 1)]
    public async Task PostCsv_DuplcateShouldReturnOkAndSuccessfullyUploadXAndShowYFailed(string fileName, int successReturnCount, int failedCount)
    {
        // Arrange
        var client = _factory.CreateClient();
        
        var projectDirectory = Directory.GetCurrentDirectory();
        var testDataFolder = Path.Combine(projectDirectory, "TestCsvData"); 
        var csvFilePath = Path.Combine(testDataFolder, fileName);

        var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(File.ReadAllBytes(csvFilePath));
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("text/csv");
        content.Add(fileContent, "file", fileName);
        
        List<MeterReadingDto> meterReadings = new();
        
        using (FileStream fileStream = File.OpenRead(csvFilePath))
        {
            meterReadings = CsvReaderHelper.ParseCsv(fileStream);
        }
        
        _factory.SeedAccounts(meterReadings.Select(x => x.AccountId).ToList());

        // Act
        var response = await client.PostAsync("/meter-reading-uploads", content);

        // Assert
        response.EnsureSuccessStatusCode();

        var resultContent = await response.Content.ReadAsStringAsync();
        var jsonResult = JsonSerializer.Deserialize<PostMeterReadingResult>(resultContent, _jsonSerializerOptions);
        
        if (string.IsNullOrWhiteSpace(resultContent) || jsonResult == null)
        {
            Assert.Fail("Missing data");
        }
        
        Assert.Equal(successReturnCount, jsonResult.Successful);
        Assert.Equal(failedCount, jsonResult.Failed);
    }
    
    [Theory]
    [InlineData("InvalidRowColumnDataInMeterReadings_1_20240511.csv", 4, 0)]
    public async Task PostCsv_InvalidRowColumnData_ShouldReturnOkAndSuccessfullyUploadXAndShowYFailed(string fileName, int successReturnCount, int failedCount)
    {
        // Arrange
        var client = _factory.CreateClient();
        
        var projectDirectory = Directory.GetCurrentDirectory();
        var testDataFolder = Path.Combine(projectDirectory, "TestCsvData");
        var csvFilePath = Path.Combine(testDataFolder, fileName);

        var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(File.ReadAllBytes(csvFilePath));
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("text/csv");
        content.Add(fileContent, "file", fileName);

        List<MeterReadingDto> meterReadings = new();
        
        using (FileStream fileStream = File.OpenRead(csvFilePath))
        {
            meterReadings = CsvReaderHelper.ParseCsv(fileStream);
        }
        
        _factory.SeedAccounts(meterReadings.Select(x => x.AccountId).ToList());
        
        // Act
        var response = await client.PostAsync("/meter-reading-uploads", content);

        // Assert
        response.EnsureSuccessStatusCode();

        var resultContent = await response.Content.ReadAsStringAsync();
        var jsonResult = JsonSerializer.Deserialize<PostMeterReadingResult>(resultContent, _jsonSerializerOptions);
        
        if (string.IsNullOrWhiteSpace(resultContent) || jsonResult == null)
        {
            Assert.Fail("Missing data");
        }
        
        Assert.Equal(successReturnCount, jsonResult.Successful);
        Assert.Equal(failedCount, jsonResult.Failed);
    }

    [Fact]
    public async Task FailAddDuplicateRecordAlreadyInDatabase()
    {
        var client = _factory.CreateClient();

        int accountId = -1123;
        DateTime readingDateTime = new DateTime(2019,04,22,11,25,00);
        int meterReading = 1002;
        
        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<SolidMReaderContext>();
            
            if (!dbContext.Accounts.Any(x => x.AccountId == accountId))
            {
                var seedAccount = new Account()
                {
                    AccountId = accountId,
                    FirstName = "Darth",
                    LastName = "Vader"
                };
                dbContext.Accounts.Add(seedAccount);
            }

            if (!dbContext.MeterReadings.Any(x=> x.AccountId == accountId && x.MeterReadValue == meterReading))
            {
                var seedReading = new MeterReading()
                {
                    MeterReadingGuid = Guid.NewGuid(), 
                    AccountId = accountId, 
                    MeterReadValue = meterReading,
                    MeterReadingDateTime = readingDateTime
                };
                dbContext.MeterReadings.Add(seedReading);
            }
            
            await dbContext.SaveChangesAsync();
        }
        
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("AccountId,MeterReadingDateTime,MeterReadValue");
        sb.AppendLine($"{accountId},{readingDateTime.ToString("dd/MM/yyyy HH:mm")},{meterReading:D5}");
        var content = new MultipartFormDataContent();

        var fileContent = new StringContent(sb.ToString(), Encoding.UTF8, "text/csv");
        fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
        {
            Name = "file",
            FileName = "test.csv"
        };
        content.Add(fileContent);

        // Act
        var response = await client.PostAsync("/meter-reading-uploads", content);
        
        //Assert
        var resultContent = await response.Content.ReadAsStringAsync();
        var jsonResult = JsonSerializer.Deserialize<PostMeterReadingResult>(resultContent, _jsonSerializerOptions);
        
        if (string.IsNullOrWhiteSpace(resultContent) || jsonResult == null)
        {
            Assert.Fail("Missing data");
        }
        
        Assert.Equal(0, jsonResult.Successful);
        Assert.Equal(1, jsonResult.Failed);
        
        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<SolidMReaderContext>();

            var readingsCount = await dbContext.MeterReadings.Where(x => x.AccountId == accountId && x.MeterReadValue == meterReading).CountAsync();
            
            Assert.Equal(1, readingsCount);
        }
    }
    
    [Theory]
    [InlineData("LowerThanLastReadingMeterReadings_20240511.csv", 0, 2)]
    public async Task PostCsv_LowerThanLastReading(string fileName, int successReturnCount, int failedCount)
    {
        // Arrange
        var client = _factory.CreateClient();
        
        var projectDirectory = Directory.GetCurrentDirectory();
        var testDataFolder = Path.Combine(projectDirectory, "TestCsvData");
        var csvFilePath = Path.Combine(testDataFolder, fileName);

        var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(File.ReadAllBytes(csvFilePath));
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("text/csv");
        content.Add(fileContent, "file", fileName);

        List<MeterReadingDto> meterReadings = new();
        
        using (FileStream fileStream = File.OpenRead(csvFilePath))
        {
            meterReadings = CsvReaderHelper.ParseCsv(fileStream);
        }
        
        _factory.SeedAccounts(meterReadings.Select(x => x.AccountId).ToList());
        _factory.SeedLowerReadings(meterReadings.ToList());
        
        // Act
        var response = await client.PostAsync("/meter-reading-uploads", content);

        // Assert
        response.EnsureSuccessStatusCode();

        var resultContent = await response.Content.ReadAsStringAsync();
        var jsonResult = JsonSerializer.Deserialize<PostMeterReadingResult>(resultContent, _jsonSerializerOptions);
        
        if (string.IsNullOrWhiteSpace(resultContent) || jsonResult == null)
        {
            Assert.Fail("Missing data");
        }
        
        Assert.Equal(successReturnCount, jsonResult.Successful);
        Assert.Equal(failedCount, jsonResult.Failed);
    }
    
    //ToDo : handel lower than last reading check
    //ToDo : add in memory db to test
    //ToDo : check for adding the correct values to DB only on POST
}