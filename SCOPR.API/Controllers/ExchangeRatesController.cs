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
                throw new ArgumentNullException(nameof(request));
            }
            // Validate the base currency
            if (string.IsNullOrWhiteSpace(request.BaseCurrency))
            {
                throw new ArgumentException("Base currency must be provided.");
            }
            // Validate the target currencies
            if (request.TargetCurrencies == null || !request.TargetCurrencies.Any())
            {
                throw new ArgumentException("At least one target currency must be provided.");
            }

            var command = new FetchExchangeRatesCommand(
                request.BaseCurrency,
                request.TargetCurrencies
            );

            // Send the command to the mediator
            await mediator.Send(command);

            // Return 
            return Ok(new { Message = "Exchange rates fetched successfully." });
        }

        [HttpGet("latest/{baseCurrency}/{targetCurrency}")]
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

        } 
    }
}
