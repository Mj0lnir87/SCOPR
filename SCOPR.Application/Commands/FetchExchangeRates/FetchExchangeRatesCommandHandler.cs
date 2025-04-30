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
        //get exchange rate
        var exchangeRateDto = await _exchangeRateApiClient.GetLatestRatesAsync(
            request.BaseCurrency,
            request.TargetCurrencies
        );
        
        //save every exchange rate
        foreach (var targetCurrency in request.TargetCurrencies)
        {
            var exchangeRate = new ExchangeRate
            {
                BaseCurrencyCode = request.BaseCurrency,
                TargetCurrencyCode = targetCurrency,
                Rate = exchangeRateDto.Rate,
                CreatedAt = DateTime.UtcNow
            };

            //save to database
            var existingExchangeRate = await _exchangeRateRepository.GetLatestRateAsync(request.BaseCurrency, targetCurrency);
            if (existingExchangeRate == null)
            {
                await _exchangeRateRepository.AddAsync(exchangeRate);
            }
            else
            {
                //update existing exchange rate with new information
                existingExchangeRate.Rate = exchangeRate.Rate;
                existingExchangeRate.CreatedAt = DateTime.UtcNow;
                await _exchangeRateRepository.AddAsync(existingExchangeRate);
            }
        }
        return Unit.Value;
    }
}