using MongoDB.Driver;
using SCOPR.Application.Interfaces;
using SCOPR.Domain.Entities;

namespace SCOPR.Infrastructure.Data.Repositories;

public class CountryRepo : ICountryRepository
{
    private readonly DbContext _dbContext;

    public async Task AddAsync(Country country)
    {
        await _dbContext.GetCollection<Country>("Countries").InsertOneAsync(country);
    }

    public async Task<List<Country>> GetAllByCodesAsync(IList<string> codes)
    {
        var filter = Builders<Country>.Filter.In(c => c.Code, codes);
        return await _dbContext.GetCollection<Country>("Countries").Find(filter).ToListAsync();
    }

    public async Task<Country> GetByCodeAsync(string code)
    {
        var filter = Builders<Country>.Filter.Eq(c => c.Code, code);
        return await _dbContext.GetCollection<Country>("Countries").Find(filter).FirstOrDefaultAsync();
    }

    public async Task<CountrySummary> GetSummaryInPeriodAsync(string code, DateTime start, DateTime end)
    {
        var filter = Builders<CountrySummary>.Filter.And(
            Builders<CountrySummary>.Filter.Eq(c => c.Currency.Code, code),
            Builders<CountrySummary>.Filter.Gte(c => c.StartDate, start),
            Builders<CountrySummary>.Filter.Lte(c => c.EndDate, end)
        );
        return await _dbContext.GetCollection<CountrySummary>("Countries").Find(filter).FirstOrDefaultAsync();
    }

    public async Task UpdateAsync(Country country)
    {
        var filter = Builders<Country>.Filter.Eq(c => c.Id, country.Id);
        var update = Builders<Country>.Update
            .Set(c => c.Name, country.Name)
            .Set(c => c.Code, country.Code)
            .Set(c => c.Currency, country.Currency);
        await _dbContext.GetCollection<Country>("Countries").UpdateOneAsync(filter, update);
    }
}