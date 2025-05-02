using MediatR;
using Microsoft.AspNetCore.Mvc;
using SCOPR.API.Requests;
using SCOPR.Application.Commands.GenerateReports;
using Swashbuckle.AspNetCore.Annotations;

namespace SCOPR.API.Controllers
{
    /// <summary>
    /// Controller for generating reports.
    /// </summary>
    /// <param name="mediator"></param>
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController(IMediator mediator) : Controller
    {
        [HttpPost("countries/summary")]
        [SwaggerResponse(200)]
        [SwaggerResponse(400)]
        [SwaggerResponse(404)]
        [SwaggerResponse(500)]
        [SwaggerOperation(Summary = "Generate a country summary report", Description = "Generates a PDF report summarizing country data for the specified date range and country codes.")]
        public async Task<IActionResult> GenerateCountrySummaryReportAsync(GenerateReportRequest request)
        {
            // Validate the request
            if (request == null)
            {
                return BadRequest(new { Message = "Request cannot be null." });
            }
            // Validate the date range
            if (request.StartDate >= request.EndDate)
            {
                return BadRequest(new { Message = "Start date must be less than or equal to end date." });
            }
            // Validate the country codes
            if (request.CountryCodes == null || !request.CountryCodes.Any())
            {
                return BadRequest("At least one country code must be provided.");
            }
            
            var command = new GenerateReportCommand(
                request.StartDate,
                request.EndDate,
                request.CountryCodes
            );

            try
            {
                // Send the command to the mediator
                var pdfBytes = await mediator.Send(command);

                //write the pdf to project base directory
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Reports", $"CountrySummaryReport_{DateTime.Now:yyyyMMdd}.pdf");
                await System.IO.File.WriteAllBytesAsync(filePath, pdfBytes);

                // Return the file path for debugging purposes
                return Ok(new { Message = $"file written to {filePath}" });
            }
            catch (ArgumentException ex)
            {
                // Handle validation-related exceptions
                return BadRequest(new { Message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                // Handle cases where a resource is not found
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                // Handle unexpected exceptions
                return StatusCode(500, new { Message = "An unexpected error occurred.", Details = ex.Message });
            }
        }
    }
}
