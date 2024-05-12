using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using SolidMReader.Models.ViewModels;

namespace SolidMReader.Test.IntegrationTests;

public class MeterReadingPost(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    [Theory]
    [InlineData("VaildMeterReadings_3_20240511.csv", 3)]
    [InlineData("VaildMeterReadings_5_20240511.csv", 5)]
    public async Task PostCsv_ShouldReturnOkAndSuccessfullyUploadX(string fileName, int successReturnCount)
    {
        // Arrange
        var client = factory.CreateClient();
        
        var projectDirectory = Directory.GetCurrentDirectory(); // Gets the output directory
        var testDataFolder = Path.Combine(projectDirectory, "TestCsvData"); // Path to your TestData folder
        var csvFilePath = Path.Combine(testDataFolder, fileName); // Replace with your CSV file name

        var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(File.ReadAllBytes(csvFilePath));
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("text/csv");
        content.Add(fileContent, "file", fileName);

        // Act
        var response = await client.PostAsync("/meter-reading-uploads", content);

        // Assert
        response.EnsureSuccessStatusCode();

        var resultContent = await response.Content.ReadAsStringAsync();
        var jsonResult = JsonSerializer.Deserialize<PostMeterReadingResult>(resultContent);
        
        if (string.IsNullOrWhiteSpace(resultContent) || jsonResult != null)
        {
            Assert.Fail("Missing data");
        }
        
        Assert.Equal(successReturnCount, jsonResult.Successful);
    }
    
    [Theory]
    [InlineData("DuplicateMeterReadings_20240511.csv", 5, 1)]
    public async Task PostCsv_DuplcateShouldReturnOkAndSuccessfullyUploadXAndShowYFailed(string fileName, int successReturnCount, int failedCount)
    {
        // Arrange
        var client = factory.CreateClient();
        
        var projectDirectory = Directory.GetCurrentDirectory(); // Gets the output directory
        var testDataFolder = Path.Combine(projectDirectory, "TestCsvData"); // Path to your TestData folder
        var csvFilePath = Path.Combine(testDataFolder, fileName); // Replace with your CSV file name

        var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(File.ReadAllBytes(csvFilePath));
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("text/csv");
        content.Add(fileContent, "file", fileName);

        // Act
        var response = await client.PostAsync("/meter-reading-uploads", content);

        // Assert
        response.EnsureSuccessStatusCode();

        var resultContent = await response.Content.ReadAsStringAsync();
        var jsonResult = JsonSerializer.Deserialize<PostMeterReadingResult>(resultContent);
        
        if (string.IsNullOrWhiteSpace(resultContent) || jsonResult != null)
        {
            Assert.Fail("Missing data");
        }
        
        Assert.Equal(successReturnCount, jsonResult.Successful);
        Assert.Equal(failedCount, jsonResult.Failed);
    }
    
    //ToDo : handel missing account
    //ToDo : add in memory db to test
    //ToDo : check for adding the correcnt values to DB only on POST
}