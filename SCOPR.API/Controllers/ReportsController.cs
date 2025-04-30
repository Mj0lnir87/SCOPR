using MediatR;
using Microsoft.AspNetCore.Mvc;
using SCOPR.API.Requests;
using SCOPR.Application.Commands.GenerateReports;

namespace SCOPR.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController(IMediator mediator) : Controller
    {
        [HttpPost("countries/summary")]
        public async Task<IActionResult> GenerateCountrySummaryReportAsync(GenerateReportRequest request)
        {
            // Validate the request
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            // Validate the date range
            if (request.StartDate > request.EndDate)
            {
                throw new ArgumentException("Start date must be less than or equal to end date.");
            }
            // Validate the country codes
            if (request.CountryCodes == null || !request.CountryCodes.Any())
            {
                throw new ArgumentException("At least one country code must be provided.");
            }


            var command = new GenerateReportCommand(
                request.StartDate,
                request.EndDate,
                request.CountryCodes
            );

            // Send the command to the mediator
            var pdfBytes = await mediator.Send(command);

            //write the pdf to project base directory
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Reports", $"CountrySummaryReport_{DateTime.Now:yyyyMMdd}.pdf");
            await System.IO.File.WriteAllBytesAsync(filePath, pdfBytes);
            
            // Return the file path for debugging purposes
            return Ok(new { Message = $"file written to {filePath}" });
        }
    }
}
