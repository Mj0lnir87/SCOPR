using SCOPR.Domain.Entities;

namespace SCOPR.Application.Interfaces;

public interface IExchangeRateRepository
{
    Task<ExchangeRate> GetLatestRateAsync(string baseCurrency, string targetCurrency, DateTime Date);

    Task<decimal> GetAverageRateInPeriodAsync(string baseCurrency, string targetCurrency, DateTime start, DateTime end);

    Task AddAsync(ExchangeRate rate);

    Task UpdateAsync(ExchangeRate rate);
}