using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SolidMReader.Data.Context;
using SolidMReader.Models.DTO;
using SolidMReader.Models.Extensions;
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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> EnterMeterReadings(IFormFile? file)
    {
        if (file == null)
        {
            _logger.LogError("Invalid File Upload");
            var error = new ErrorResponse() { ErrorCode = 10001, ErrorDescription = "Invalid file" };

            return BadRequest(error);
        }
        
        try
        {
            ProcessCsvResult recordsToProcess = _meterReadingsProcessor.ProcessCsvToMeterReadings(file);

            var readingsValidator = recordsToProcess.ValidMeterReadings.ValidateReadings(_validateReadings);

            var savedValidMeterReadings = await _meterReadingsRepository.AddValidReadings(readingsValidator.ValidReadings);

            PostMeterReadingResult output = new()
            {
                Failed = readingsValidator.FailedReadings.Count + recordsToProcess.FailedToParse.Count
            };

            output.FailedReadings.AddRange(recordsToProcess.FailedToParse);
            output.FailedReadings.AddRange(readingsValidator.FailedReadings.Select(x => $"{x.AccountId} : {x.MeterReadingDateTime} : {x.MeterReadValue}").ToList());
            
            if (savedValidMeterReadings)
            {
                output.Successful = readingsValidator.ValidReadings.Count;
            }
            else
            {
                output.Failed += readingsValidator.ValidReadings.Count;
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