using System.Globalization;
using System.Net.Http.Headers;
using CsvHelper;
using CsvHelper.Configuration;
using SolidMReader.Models.DTO;

namespace SolidMReader.Test.Helper;

public static class CsvReaderHelper
{
    public static List<MeterReading> ParseCsv(Stream file)
    {
        List<MeterReading> output = new();
        using var reader = new StreamReader(file);

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            MissingFieldFound = null
        };

        try
        {
            using (var csv = new CsvReader(reader, config))
            {
                output = csv.GetRecords<MeterReading>().ToList();
            }
        }
        catch (Exception e)
        {
            throw new Exception("CsvReaderHelper failed to parse CSV", e);
        }

        return output;
    }

    public static string GetCsvFilePath(string fileName)
    {
        var projectDirectory = Directory.GetCurrentDirectory();
        var testDataFolder = Path.Combine(projectDirectory, "TestCsvData");
        return Path.Combine(testDataFolder, fileName);
    }
    
    public static MultipartFormDataContent GetCsvContent(string csvFilePath, string fileName)
    {
        var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(File.ReadAllBytes(csvFilePath));
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("text/csv");
        content.Add(fileContent, "file", fileName);

        return content;
    }
}