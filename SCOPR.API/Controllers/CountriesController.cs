using MediatR;
using Microsoft.AspNetCore.Mvc;
using SCOPR.API.DTOs;
using SCOPR.API.Requests;
using SCOPR.Application.Commands.FetchCountries;
using SCOPR.Application.Queries.GetCountrySummary;

namespace SCOPR.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CountriesController(IMediator mediator) : Controller
    {
        [HttpPost("fetch")]
        public async Task<IActionResult> FetchCountriesAsync(FetchCountriesRequest request)
        {
            // Validate the request
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            // Validate the country codes
            if (request.CountryCodes == null || !request.CountryCodes.Any())
            {
                throw new ArgumentException("At least one country code must be provided.");
            }

            var command = new FetchCountriesCommand(
                request.CountryCodes
            );

            // Send the command to the mediator
            await mediator.Send(command);

            // Return 
            return Ok(new { Message = "Countries fetched successfully." });

        }

        [HttpGet("summary/{countryCode}")]
        public async Task<ActionResult<CountryDto>> GetCountryAsync(string code, DateTime startDate, DateTime endDate)
        {
            // Validate the request
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new ArgumentNullException(nameof(code));
            }

            // Validate the date range
            if (startDate > endDate)
            {
                throw new ArgumentException("Start date must be less than or equal to end date.");
            }

            // Fetch the country summary from the repository
            var country = await mediator.Send(new GetCountrySummaryQuery(code, startDate, endDate));
            if (country == null)
            {
                return NotFound();
            }
            return Ok(country);

        }

        [HttpGet("summary/{countryCode}/period")]
        public async Task<ActionResult<CountrySummaryDto>> GetCountrySummaryAsync(string code, DateTime startDate, DateTime endDate)
        {      
            // Validate the request
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new ArgumentNullException(nameof(code));
            }            
            
            // Validate the date range
            if (startDate > endDate)
            {
                throw new ArgumentException("Start date must be less than or equal to end date.");
            }

            // Fetch the country summary from the repository
            var countrySummary = await mediator.Send(new GetCountrySummaryQuery(code, startDate, endDate));
            if (countrySummary == null)
            {
                return NotFound();
            }
            return Ok(countrySummary);

        }
    }
}
