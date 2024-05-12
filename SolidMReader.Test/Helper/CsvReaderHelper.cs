using System.Globalization;
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
}