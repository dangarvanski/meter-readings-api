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

    //[HttpPost("/meter-reading-uploads")]
    //[ProducesResponseType(StatusCodes.Status200OK)]
    //public async Task<IActionResult> UploadCsvFile(IFormFile file)
    //{
    //    // Check if a file was uploaded
    //    if (file == null || file.Length == 0)
    //    {
    //        return BadRequest("No file uploaded.");
    //    }

    //    // Check if the file is a CSV
    //    if (!file.FileName.EndsWith(".csv"))
    //    {
    //        return BadRequest("Please upload a CSV file.");
    //    }

    //    try
    //    {
    //        // Read the CSV file
    //        var records = new List<string[]>();
    //        using (var stream = new StreamReader(file.OpenReadStream(), Encoding.UTF8))
    //        {
    //            while (!stream.EndOfStream)
    //            {
    //                var line = await stream.ReadLineAsync();
    //                var values = line.Split(','); // Basic CSV parsing (assumes no commas in fields)
    //                records.Add(values);
    //            }
    //        }

    //        // Process the CSV data (example: return the records)
    //        return Ok(new
    //        {
    //            Message = "CSV file processed successfully.",
    //            RecordCount = records.Count,
    //            Records = records
    //        });
    //    }
    //    catch (Exception ex)
    //    {
    //        return StatusCode(500, $"Error processing CSV file: {ex.Message}");
    //    }
    //}
}
