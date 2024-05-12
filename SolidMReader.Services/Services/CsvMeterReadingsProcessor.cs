using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;
using SolidMReader.Models.DTO;
using SolidMReader.Services.Interfaces;

namespace SolidMReader.Services.Services;

public class CsvMeterReadingsProcessor : ICsvMeterReadingsProcessor
{
    public List<MeterReading> ProcessCsvToMeterReadings(IFormFile file)
    {
        List<MeterReading> output = new ();
        
        using var reader = new StreamReader(file.OpenReadStream());
            
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
            throw e;
        }

        return output;
    }
}