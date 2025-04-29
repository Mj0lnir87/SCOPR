using SCOPR.API.DTOs;
using SCOPR.Application.Interfaces;

namespace SCOPR.Infrastructure.Services;

public class FixerApiClient : IExchangeRateApiClient
{
    public Task<ExchangeRateDto> GetLatestRatesAsync(string baseCurrency, List<string> targetCurrencies)
    {
        throw new NotImplementedException();
    }
}