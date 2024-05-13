using System.Globalization;
using System.Net.Http.Headers;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using SolidMReader.Models.DTO;
using SolidMReader.Services.Validation;

namespace SolidMReader.Test.Helper;

public static class CsvReaderHelper
{
    public static List<MeterReading> ParseCsv(Stream file)
    {
        List<MeterReading> output = new();
        using var reader = new StreamReader(file);

        var badData = new List<string>();
        
        var isRecordBad = false;
        
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            MissingFieldFound = null,
            BadDataFound = 
                b =>
                {
                    isRecordBad = true;
                    Console.WriteLine($"Bad data found on row {b.RawRecord}: {b.Field}");
            },
            ReadingExceptionOccurred = e =>
            {
                isRecordBad = true;
                return true;
            }
        };

        try
        {
            using (var csv = new CsvReader(reader, config))
            {
                while (csv.Read())
                {
                    try
                    {
                        var record = csv.GetRecord<MeterReading>();

                        if (!isRecordBad)
                        {
                            output.Add(record);
                        }
                    }
                    catch (TypeConverterException ex)
                    {
                        if (ex.MemberMapData.Member.Name == "MeterReadValue")
                        {
                            Console.WriteLine(ex);
                        }
                        else
                        {
                            var t = 42;
                        }
                    }
                    finally
                    {
                        isRecordBad = false;
                    }
                }
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