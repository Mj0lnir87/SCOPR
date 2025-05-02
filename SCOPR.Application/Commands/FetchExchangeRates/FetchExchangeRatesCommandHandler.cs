using MediatR;
using SCOPR.Application.Interfaces;
using SCOPR.Domain.Entities;

namespace SCOPR.Application.Commands.FetchExchangeRates;

public class FetchExchangeRatesCommandHandler : IRequestHandler<FetchExchangeRatesCommand, Unit>
{
    private readonly IExchangeRateApiClient _exchangeRateApiClient;
    private readonly IExchangeRateRepository _exchangeRateRepository;

    public FetchExchangeRatesCommandHandler(
        IExchangeRateApiClient exchangeRateApiClient,
        IExchangeRateRepository exchangeRateRepository)
    {
        _exchangeRateApiClient = exchangeRateApiClient;
        _exchangeRateRepository = exchangeRateRepository;
    }

    public async Task<Unit> Handle(FetchExchangeRatesCommand request, CancellationToken cancellationToken)
    {
        // Get exchange rate
        var exchangeRateDtos = await _exchangeRateApiClient.GetLatestRatesAsync(
            request.BaseCurrency,
            request.TargetCurrencies,
            request.StartDate,
            request.EndDate
        );

        // Loop all dates in the range
        foreach (var exchangeRateDto in exchangeRateDtos)
        {
            // Save every exchange rate
            foreach (var targetCurrency in request.TargetCurrencies)
            {
                // Get exchange rate for the target currency
                var targetCurrencyRate = exchangeRateDto.rates[targetCurrency.ToUpperInvariant()];
                if (targetCurrencyRate == null)
                {
                    throw new KeyNotFoundException($"Exchange rate not found for target currency: {targetCurrency}");
                }

                var exchangeRate = new ExchangeRate
                {
                    BaseCurrencyCode = request.BaseCurrency,
                    TargetCurrencyCode = targetCurrency.ToUpperInvariant(),
                    Rate = (decimal)targetCurrencyRate,
                    Date = exchangeRateDto.date,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                // Save to database
                var existingExchangeRate = await _exchangeRateRepository.GetLatestRateAsync(request.BaseCurrency, targetCurrency.ToUpperInvariant(), exchangeRateDto.date);
                if (existingExchangeRate == null)
                {
                    await _exchangeRateRepository.AddAsync(exchangeRate);
                }
                else
                {
                    // Update existing exchange rate with new information
                    existingExchangeRate.Rate = exchangeRate.Rate;
                    existingExchangeRate.UpdatedAt = DateTime.Now;
                    await _exchangeRateRepository.UpdateAsync(existingExchangeRate);
                }
            }
        }

        return Unit.Value;
    }
}