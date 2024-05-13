using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SolidMReader.Models.DTO;
using SolidMReader.Services.Interfaces;

namespace SolidMReader.Services.Services;

public class CsvMeterReadingsProcessor : ICsvMeterReadingsProcessor
{
    private readonly ILogger<CsvMeterReadingsProcessor> _logger;

    public CsvMeterReadingsProcessor(ILogger<CsvMeterReadingsProcessor> logger)
    {
        _logger = logger;
    }
    public ProcessCsvResult ProcessCsvToMeterReadings(IFormFile file)
    {
        ProcessCsvResult output = new ()
        {
            ValidMeterReadings = new(),
            FailedToParse = new()
        };
        
        using var reader = new StreamReader(file.OpenReadStream());
        var isRecordBad = false;
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            MissingFieldFound = null,
            BadDataFound = 
                b =>
                {
                    _logger.LogWarning($"Bad data found on row {b.RawRecord}: {b.Field}");
                },
            ReadingExceptionOccurred = e => true
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
                        output.ValidMeterReadings.Add(record);
                    }
                    catch (TypeConverterException ex)
                    {
                        output.FailedToParse.Add(ex.Text);
                    }
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error ProcessCsvToMeterReadings");
            throw e;
        }
        
        return output;
    }
}