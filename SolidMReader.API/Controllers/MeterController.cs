using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
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
    private readonly ICsvMeterReadingsProcessor _meterReadingsProcessor;

    public MeterController(ILogger<MeterController> logger, 
        IMeterReadingsRepository meterReadingsRepository, 
        IValidation<MeterReading> validateReadings,
        ICsvMeterReadingsProcessor meterReadingsProcessor)
    {
        _logger = logger;
        _meterReadingsRepository = meterReadingsRepository;
        _validateReadings = validateReadings;
        _meterReadingsProcessor = meterReadingsProcessor;
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

        try
        {
            List<MeterReading> recordsToProcess = _meterReadingsProcessor.ProcessCsvToMeterReadings(file);

            foreach (var reading in recordsToProcess)
            {
                if (_validateReadings.IsValid(reading) 
                    && !validReadings.Any(x => x.AccountId == reading.AccountId && x.MeterReadValue == reading.MeterReadValue))
                {
                    validReadings.Add(reading);
                }
                else
                {
                    failedReadings.Add(reading);
                }
            }

            var savedValidMeterReadings = await _meterReadingsRepository.AddValidReadings(validReadings);

            PostMeterReadingResult output = new()
            {
                Failed = failedReadings.Count,
                FailedReading = failedReadings.Select(x => $"{x.AccountId} : {x.MeterReadingDateTime} : {x.MeterReadValue}").ToList()
            };

            if (savedValidMeterReadings)
            {
                output.Successful = validReadings.Count;
            }
            else
            {
                output.Failed += validReadings.Count;
            }        
            
            return Ok(output);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Issue Processing csv");
            return BadRequest();
        }
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