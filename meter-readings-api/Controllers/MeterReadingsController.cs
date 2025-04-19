using MediatR;
using meter_readings_application.Commands;
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
}
