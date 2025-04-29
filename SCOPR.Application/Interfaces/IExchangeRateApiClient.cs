using SCOPR.API.DTOs;

namespace SCOPR.Application.Interfaces;

public interface IExchangeRateApiClient
{
    Task<ExchangeRateDto> GetLatestRatesAsync(string baseCurrency, List<string> targetCurrencies);
}