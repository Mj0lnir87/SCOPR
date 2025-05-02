using MongoDB.Driver;
using SCOPR.Application.Interfaces;
using SCOPR.Domain.Entities;
using System.Reflection.Emit;

namespace SCOPR.Infrastructure.Data.Repositories;

public class CountryRepo : ICountryRepository
{
    private readonly DbContext _dbContext;

    public CountryRepo(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

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
        var filter = Builders<Country>.Filter.And(
            Builders<Country>.Filter.Eq(c => c.Code, code.ToUpperInvariant()),
            Builders<Country>.Filter.Gte(c => c.CreatedAt, DateTime.Now.Date),
            Builders<Country>.Filter.Lt(c => c.CreatedAt, DateTime.Now.Date.AddDays(1))
            );
        return await _dbContext.GetCollection<Country>("Countries").Find(filter).FirstOrDefaultAsync();
    }

    public async Task UpdateAsync(Country country)
    {
        var filter = Builders<Country>.Filter.Eq(c => c.Id, country.Id);
        await _dbContext.GetCollection<Country>("Countries").ReplaceOneAsync(filter, country);
    }

    public async Task<double> GetAveragePopulationInPeriodAsync(string requestCountryCode, DateTime startDate, DateTime endDate)
    {
        var filter = Builders<Country>.Filter.And(
            Builders<Country>.Filter.Eq(c => c.Code, requestCountryCode.ToUpperInvariant()),
            Builders<Country>.Filter.Gte(c => c.CreatedAt, startDate.Date),
            Builders<Country>.Filter.Lt(c => c.CreatedAt, endDate.Date.AddDays(1))
        );
        var population =  await _dbContext.GetCollection<Country>("Countries").Find(filter).ToListAsync();
        return population.Average(p => p.Population);

    }
}