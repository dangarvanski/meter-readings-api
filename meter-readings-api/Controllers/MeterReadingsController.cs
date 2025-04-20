using MediatR;
using meter_reading_sharedKernal.Entities;
using meter_readings_application.Commands;
using meter_readings_application.Queries;
using Microsoft.AspNetCore.Mvc;

namespace meter_readings_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MeterReadingsController : ControllerBase
{
    private readonly IMediator _mediator;

    public MeterReadingsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("/all-records")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<MeterReadingDbRecord>>> GetAllRecords([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var records = await _mediator.Send(new GetAllRecordsQuery(page, pageSize));

        if (records.Count == 0)
        {
            return NotFound("No records have been found!");
        }

        return Ok(records);
    }

    [HttpGet("/records-for-account{accountId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<MeterReadingDbRecord>>> GetRecordById(int accountId)
    {
        var response = await _mediator.Send(new GetRecordsByAccountIdQuery(accountId));

        if (!response.Success)
        {
            return NotFound(response.Message);
        }

        return Ok(response.MeterReadings);
    }

    [HttpPost("/meter-reading-uploads")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadCsvFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        if (!file.FileName.EndsWith(".csv"))
        {
            return BadRequest("Please upload a CSV file.");
        }

        var result = await _mediator.Send(new UploadCsvCommand(file));

        if (result.IsSuccess)
        {
            return Ok(new
            {
                result.Message,
                result.SuccessRecordCount,
                result.FailedRecordCount
            });
        }

        return BadRequest(new { result.Message });
    }

    [HttpDelete("/empty-database/{confirmation}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<bool>> EmptyDatabase(string confirmation)
    {
        var result = await _mediator.Send(new EmptyDatabaseCommand(confirmation));

        if (result == false)
        {
            return BadRequest($"Invalid confirmation statement!");
        }

        return Ok($"Records database has been emptied.");
    }
}
