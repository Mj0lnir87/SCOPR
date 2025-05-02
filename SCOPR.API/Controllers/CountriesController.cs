using MediatR;
using Microsoft.AspNetCore.Mvc;
using SCOPR.Application.DTOs;
using SCOPR.API.Requests;
using SCOPR.Application.Commands.FetchCountries;
using SCOPR.Application.Queries.GetCountrySummary;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using SCOPR.Domain.Entities;
using SCOPR.Application.Queries.GetCountriesSummary;

namespace SCOPR.API.Controllers
{
    /// <summary>
    /// Controller for managing countries.
    /// </summary>
    /// <param name="mediator"></param>
    [ApiController]
    [Route("api/[controller]")]
    public class CountriesController(IMediator mediator) : Controller
    {
        /// <summary>
        /// Fetches countries from the API based on the provided country codes.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Gets the summary of a country based on the provided country code and date range.
        /// </summary>
        /// <param name="countryCode"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [HttpGet("summary/{countryCode}")]
        [SwaggerResponse(200)]
        [SwaggerResponse(400)]
        [SwaggerResponse(404)]
        [SwaggerResponse(500)]
        [SwaggerOperation(Summary = "Get country summary", Description = "Fetches the summary of a country based on the provided country code and date range.")]
        public async Task<ActionResult<CountrySummary>> GetCountryAsync(string countryCode, DateTime startDate, DateTime endDate)
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

        /// <summary>
        /// Gets the summaries of all countries based on the provided date range.
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [HttpGet("summary")]
        [SwaggerResponse(200)]
        [SwaggerResponse(400)]
        [SwaggerResponse(404)]
        [SwaggerResponse(500)]
        [SwaggerOperation(Summary = "Get all countries summary", Description = "Fetches the summaries of all countries based on the provided date range.")]
        public async Task<ActionResult<IList<CountrySummary>>> GetCountriesAsync(DateTime startDate, DateTime endDate)
        {
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
                var countriesSymmary = await mediator.Send(new GetCountriesSummaryQuery(startDate, endDate));
                if (countriesSymmary == null)
                {
                    return NotFound();
                }

                return Ok(countriesSymmary);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }

        }
    }
}
