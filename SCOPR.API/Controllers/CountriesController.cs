using MediatR;
using Microsoft.AspNetCore.Mvc;
using SCOPR.API.DTOs;
using SCOPR.API.Requests;
using SCOPR.Application.Commands.FetchCountries;
using SCOPR.Application.Queries.GetCountrySummary;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace SCOPR.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CountriesController(IMediator mediator) : Controller
    {
        /// <summary>
        /// Save a person
        /// </summary>
        [HttpPost("fetch")]
        [SwaggerResponse(200)]
        [SwaggerResponse(400)]
        [SwaggerResponse(404)]
        [SwaggerResponse(500)]
        [SwaggerOperation(Summary = "Fetch countries from the API", Description = "Fetches countries from the 'REST Countries' API based on the provided country codes.")]
        public async Task<IActionResult> FetchCountriesAsync(FetchCountriesRequest request)
        {
            // Validate the request
            if (request == null)
            {
                return BadRequest(new { Message = "Request cannot be null." });
            }

// \UValidate the country codes
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
        [SwaggerResponse(200)]
        [SwaggerResponse(400)]
        [SwaggerResponse(404)]
        [SwaggerResponse(500)]
        [SwaggerOperation(Summary = "Get country summary", Description = "Fetches the summary of a country based on the provided country code and date range.")]
        public async Task<ActionResult<CountryDto>> GetCountryAsync(string countryCode, DateTime startDate, DateTime endDate)
        {
            // Validate the request
            if (string.IsNullOrWhiteSpace(countryCode))
            {
                return BadRequest(new { Message = "Country code cannot be null or empty." });
            }

            // Validate the date range
            if (startDate >= endDate)
            {
                return BadRequest(new { Message = "Start date must be less than or equal to end date." });
            }

            // The date range should be within one month
            if ((endDate - startDate).TotalDays > 30)
            {
                return BadRequest(new { Message = "Date range must be less than or equal to 30 days." });
            }

            // End date should be before or equal to today
            if (endDate > DateTime.Now)
            {
                return BadRequest(new { Message = "End date must be less than or equal to today." });
            }

            try
            {
                // Fetch the country summary from the repository
                var country = await mediator.Send(new GetCountrySummaryQuery(countryCode, startDate, endDate));
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
    }
}
