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

    public async Task<ExchangeRate> GetLatestRateAsync(string baseCurrency, string targetCurrency)
    {
        var filter = Builders<ExchangeRate>.Filter.And(
            Builders<ExchangeRate>.Filter.Eq(r => r.BaseCurrencyCode, baseCurrency),
            Builders<ExchangeRate>.Filter.Eq(r => r.TargetCurrencyCode, targetCurrency)
        );
        return await _dbContext.GetCollection<ExchangeRate>("ExchangeRates").Find(filter).FirstOrDefaultAsync();
    }

    public async Task<decimal> GetAverageRateInPeriodAsync(string baseCurrency, string targetCurrency, DateTime start, DateTime end)
    {
        var filter = Builders<ExchangeRate>.Filter.And(
            Builders<ExchangeRate>.Filter.Eq(r => r.BaseCurrencyCode, baseCurrency),
            Builders<ExchangeRate>.Filter.Eq(r => r.TargetCurrencyCode, targetCurrency),
            Builders<ExchangeRate>.Filter.Gte(r => r.CreatedAt, start),
            Builders<ExchangeRate>.Filter.Lte(r => r.CreatedAt, end)
        );
        var exchangeRates = await _dbContext.GetCollection<ExchangeRate>("ExchangeRates").Find(filter).ToListAsync();
        return exchangeRates.Average(r => r.Rate);
    }

    public async Task AddAsync(ExchangeRate rate)
    {
        await _dbContext.GetCollection<ExchangeRate>("ExchangeRates").InsertOneAsync(rate);
    }
}