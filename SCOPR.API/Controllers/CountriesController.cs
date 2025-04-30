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
                return BadRequest(new { Message = "Request cannot be null." });
            }

            // Validate the country codes
            if (request.CountryCodes == null || !request.CountryCodes.Any())
            {
                return BadRequest(new { Message = "At least one country code must be provided." });
            }

            var command = new FetchCountriesCommand(
                request.CountryCodes
            );

            try
            {
                // Send the command to the mediator
                await mediator.Send(command);

                // Return 
                return Ok(new { Message = "Countries fetched successfully." });
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

        [HttpGet("summary/{countryCode}")]
        public async Task<ActionResult<CountryDto>> GetCountryAsync(string code, DateTime startDate, DateTime endDate)
        {
            // Validate the request
            if (string.IsNullOrWhiteSpace(code))
            {
                return BadRequest(new { Message = "Country code cannot be null or empty." });
            }

            // Validate the date range
            if (startDate > endDate)
            {
                return BadRequest(new { Message = "Start date must be less than or equal to end date." });
            }

            try
            {
                // Fetch the country summary from the repository
                var country = await mediator.Send(new GetCountrySummaryQuery(code, startDate, endDate));
                if (country == null)
                {
                    return NotFound();
                }
                return Ok(country);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }

        }

        [HttpGet("summary/{countryCode}/period")]
        public async Task<ActionResult<CountrySummaryDto>> GetCountrySummaryAsync(string code, DateTime startDate, DateTime endDate)
        {      
            // Validate the request
            if (string.IsNullOrWhiteSpace(code))
            {
                return BadRequest(new { Message = "Country code cannot be null or empty." });
            }            
            
            // Validate the date range
            if (startDate > endDate)
            {
                return BadRequest(new { Message = "Start date must be less than or equal to end date." });
            }

            try
            {
                // Fetch the country summary from the repository
                var countrySummary = await mediator.Send(new GetCountrySummaryQuery(code, startDate, endDate));
                if (countrySummary == null)
                {
                    return NotFound();
                }
                return Ok(countrySummary);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }


        }
    }
}
