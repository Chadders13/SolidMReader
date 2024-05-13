
using Microsoft.AspNetCore.Http;
using SolidMReader.Models.DTO;

namespace SolidMReader.Services.Interfaces;

public interface ICsvMeterReadingsProcessor
{
    ProcessCsvResult ProcessCsvToMeterReadings(IFormFile file);
}
