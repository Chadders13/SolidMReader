using System.Globalization;
using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using SolidMReader.Data.Context;
using SolidMReader.Models.DTO;
using SolidMReader.Models.ViewModels;
using SolidMReader.Services.Interfaces;

namespace SolidMReader.API.Controllers;

[ApiController]
[Route("")] 
public class MeterController : ControllerBase
{
    private readonly ILogger<MeterController> _logger;
    private readonly IMeterReadingsRepository _meterReadingsRepository;
    private readonly IValidation<MeterReading> _validateReadings;

    public MeterController(ILogger<MeterController> logger, IMeterReadingsRepository meterReadingsRepository, IValidation<MeterReading> validateReadings)
    {
        _logger = logger;
        _meterReadingsRepository = meterReadingsRepository;
        _validateReadings = validateReadings;
    }
    
    [HttpPost("meter-reading-uploads")]
    public async Task<IActionResult> EnterMeterReadings(IFormFile? file)
    {
        if (file == null)
        {
            _logger.LogError("Invalid File Upload");
            var error = new ErrorResponse() { ErrorCode = 10001, ErrorDescription = "Invalid file" };

            return BadRequest(error);
        }
        List<MeterReading> validReadings = new ();
        List<MeterReading> failedReadings = new ();
        
        List<MeterReading> recordsToProcess = new ();
        try
        {
            using var reader = new StreamReader(file.OpenReadStream());
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                recordsToProcess = csv.GetRecords<MeterReading>().ToList();

            }

            foreach (var reading in recordsToProcess)
            {
                if (_validateReadings.IsValid(reading))
                {
                    validReadings.Add(reading);
                }
                else
                {
                    failedReadings.Add(reading);
                }
            }

            var output = await _meterReadingsRepository.AddValidReadings(validReadings);

            return Ok(output);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Issue Processing csv");
        }
        //ToDo : validate each reading for the insert

        
        
        var result = new PostMeterReadingResult();
        return Ok(result);
    }

    [HttpGet("readings/{id}")]
    public async Task<IActionResult> GetReading([FromRoute]Guid id)
    {
        var readingsResult = _meterReadingsRepository.GetMeterReading(id);
        
        return Ok(readingsResult);
    }
    
    [HttpGet("Account/{id}/lastReading")]
    public async Task<IActionResult> GetAccountLastReading([FromRoute]int id)
    {
        var readingsResult = await _meterReadingsRepository.GetAccountLastMeterReading(id);
        
        return Ok(readingsResult);
    }
}