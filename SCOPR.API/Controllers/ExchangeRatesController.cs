using MediatR;
using Microsoft.AspNetCore.Mvc;
using SCOPR.API.DTOs;
using SCOPR.API.Requests;
using SCOPR.Application.Commands.FetchExchangeRates;
using SCOPR.Application.Interfaces;

namespace SCOPR.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExchangeRatesController(IMediator mediator, IExchangeRateRepository exchangeRateRepository) : Controller
    {

        [HttpPost("fetch")]
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

            var command = new FetchExchangeRatesCommand(
                request.BaseCurrency,
                request.TargetCurrencies
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

       /* [HttpGet("latest/{baseCurrency}/{targetCurrency}")]
        public async Task<ActionResult<ExchangeRateDto>> GetLatestExchangeRateAsync(string baseCurrency,
            string targetCurrency)
        {
            // Validate the base and target currencies
            if (string.IsNullOrWhiteSpace(baseCurrency))
            {
                throw new ArgumentNullException(nameof(baseCurrency));
            }
            if (string.IsNullOrWhiteSpace(targetCurrency))
            {
                throw new ArgumentNullException(nameof(targetCurrency));
            }

            // Fetch the latest exchange rate from the repository
            var exchangeRate = await exchangeRateRepository.GetLatestRateAsync(baseCurrency, targetCurrency);
            if (exchangeRate == null)
            {
                return NotFound();
            }

            return Ok(new ExchangeRateDto(
                exchangeRate.BaseCurrencyCode, 
                exchangeRate.TargetCurrencyCode,
                exchangeRate.Rate,
                exchangeRate.CreatedAt));

        } */
    }
}
