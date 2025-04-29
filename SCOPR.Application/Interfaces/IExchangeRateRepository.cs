using SCOPR.Domain.Entities;

namespace SCOPR.Application.Interfaces;

public interface IExchangeRateRepository
{
    Task<ExchangeRate> GetLatestRateAsync(string baseCurrency, string targetCurrency);

    Task<decimal> GetAverageRateInPeriodAsync(string baseCurrency, string targetCurrency, DateTime start, DateTime end);

    Task AddAsync(ExchangeRate rate);
}