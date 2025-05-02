using SCOPR.Application.DTOs;

namespace SCOPR.Application.Interfaces;

public interface IExchangeRateApiClient
{
    Task<IList<ExchangeRateDto>> GetLatestRatesAsync(string baseCurrency, List<string> targetCurrencies, DateTime startDate, DateTime endDate);
}