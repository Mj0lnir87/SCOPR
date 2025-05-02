using MediatR;
using Microsoft.AspNetCore.Mvc;
using SCOPR.Application.DTOs;
using SCOPR.API.Requests;
using SCOPR.Application.Commands.FetchExchangeRates;
using SCOPR.Application.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace SCOPR.API.Controllers
{
    /// <summary>
    /// Controller for managing exchange rates.
    /// </summary>
    /// <param name="mediator"></param>
    [ApiController]
    [Route("api/[controller]")]
    public class ExchangeRatesController(IMediator mediator) : Controller
    {
        /// <summary>
        /// Fetch exchange rates from the API
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("fetch")]
        [SwaggerResponse(200)]
        [SwaggerResponse(400)]
        [SwaggerResponse(404)]
        [SwaggerResponse(500)]
        [SwaggerOperation(Summary = "Fetch exchange rates from the API", Description = "Fetches exchange rates from the 'Fixer.io' API based on the provided base currency and target currencies.")]
        public async Task<IActionResult> FetchExchangeRatesAsync(FetchExchangeRatesRequest request)
        {
            // Validate the request
            if (request == null)
            {
                return BadRequest(new { Message = "Request cannot be null." });
            }
            // Validate the base currency
            if (string.IsNullOrWhiteSpace(request.BaseCurrency))
            {
                return BadRequest(new { Message = "Base currency must be provided." });
            }
            // Validate the target currencies
            if (request.TargetCurrencies == null || !request.TargetCurrencies.Any())
            {
                return BadRequest(new { Message = "At least one target currency must be provided." });
            }

            // Validate the date range
            if (request.StartDate >= request.EndDate)
            {
                return BadRequest(new { Message = "Start date must be less than or equal to end date." });
            }

            var command = new FetchExchangeRatesCommand(
                request.BaseCurrency,
                request.TargetCurrencies,
                request.StartDate,
                request.EndDate
            );

            try
            {
                // Send the command to the mediator
                await mediator.Send(command);

                // Return 
                return Ok(new { Message = "Exchange rates fetched successfully." });
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
