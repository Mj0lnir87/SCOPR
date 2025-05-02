using MongoDB.Driver;
using SCOPR.Application.Interfaces;
using SCOPR.Domain.Entities;

namespace SCOPR.Infrastructure.Data.Repositories;

public class ExchangeRateRepo : IExchangeRateRepository
{
    private readonly DbContext _dbContext;

    public ExchangeRateRepo(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ExchangeRate> GetLatestRateAsync(string baseCurrency, string targetCurrency, DateTime date)
    {
        var filter = Builders<ExchangeRate>.Filter.And(
            Builders<ExchangeRate>.Filter.Eq(r => r.BaseCurrencyCode, baseCurrency.ToUpperInvariant()),
            Builders<ExchangeRate>.Filter.Eq(r => r.TargetCurrencyCode, targetCurrency.ToUpperInvariant()),
            Builders<ExchangeRate>.Filter.Eq(r => r.Date, date)
        );
        return await _dbContext.GetCollection<ExchangeRate>("ExchangeRates").Find(filter).FirstOrDefaultAsync();
    }

    public async Task<decimal> GetAverageRateInPeriodAsync(string baseCurrency, string targetCurrency, DateTime start, DateTime end)
    {
        var filter = Builders<ExchangeRate>.Filter.And(
            Builders<ExchangeRate>.Filter.Eq(r => r.BaseCurrencyCode, baseCurrency.ToUpperInvariant()),
            Builders<ExchangeRate>.Filter.Eq(r => r.TargetCurrencyCode, targetCurrency.ToUpperInvariant()),
            Builders<ExchangeRate>.Filter.Gte(r => r.Date, start),
            Builders<ExchangeRate>.Filter.Lte(r => r.Date, end)
        );
        var exchangeRates = await _dbContext.GetCollection<ExchangeRate>("ExchangeRates").Find(filter).ToListAsync();
        return exchangeRates.Average(r => r.Rate);
    }

    public async Task AddAsync(ExchangeRate rate)
    {
        await _dbContext.GetCollection<ExchangeRate>("ExchangeRates").InsertOneAsync(rate);
    }

    public async Task UpdateAsync(ExchangeRate rate)
    {
        var filter = Builders<ExchangeRate>.Filter.Eq(r => r.Id, rate.Id);
        await _dbContext.GetCollection<ExchangeRate>("ExchangeRates").ReplaceOneAsync(filter, rate);
    }
}