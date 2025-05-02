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
    

    /// <summary>
    /// Adds a new country to the database.
    /// </summary>
    /// <param name="country"></param>
    /// <returns></returns>
    public async Task AddAsync(Country country)
    {
        await _dbContext.GetCollection<Country>("Countries").InsertOneAsync(country);
    }
    
    /// <summary>
    /// Gets a country by its code.
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    public async Task<Country> GetByCodeAsync(string code)
    {
        var filter = Builders<Country>.Filter.And(
            Builders<Country>.Filter.Eq(c => c.Code, code.ToUpperInvariant()),
            Builders<Country>.Filter.Gte(c => c.CreatedAt, DateTime.Now.Date),
            Builders<Country>.Filter.Lt(c => c.CreatedAt, DateTime.Now.Date.AddDays(1))
            );
        return await _dbContext.GetCollection<Country>("Countries").Find(filter).FirstOrDefaultAsync();
    }

    /// <summary>
    /// Gets all countries from the database.
    /// </summary>
    /// <returns></returns>
    public async Task<IList<Country>> GetAllAsync()
    {
        var countries = await _dbContext.GetCollection<Country>("Countries").Find(_ => true).ToListAsync();

        // Ensure distinct countries based on the Code property
        var distinctCountries = countries
            .GroupBy(c => c.Code.ToUpperInvariant())
            .Select(g => g.First())
            .ToList();

        return distinctCountries;
    }

    /// <summary>
    /// Gets a country by its ID.
    /// </summary>
    /// <param name="country"></param>
    /// <returns></returns>
    public async Task UpdateAsync(Country country)
    {
        var filter = Builders<Country>.Filter.Eq(c => c.Id, country.Id);
        await _dbContext.GetCollection<Country>("Countries").ReplaceOneAsync(filter, country);
    }

    /// <summary>
    /// Gets the average population of a country in a given period.
    /// </summary>
    /// <param name="requestCountryCode"></param>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <returns></returns>
    public async Task<double> GetAveragePopulationInPeriodAsync(string requestCountryCode, DateTime startDate, DateTime endDate)
    {
        var filter = Builders<Country>.Filter.And(
            Builders<Country>.Filter.Eq(c => c.Code, requestCountryCode.ToUpperInvariant()),
            Builders<Country>.Filter.Gte(c => c.CreatedAt, startDate.Date),
            Builders<Country>.Filter.Lt(c => c.CreatedAt, endDate.Date.AddDays(1))
        );
        var population =  await _dbContext.GetCollection<Country>("Countries").Find(filter).ToListAsync();
        return population.Count > 0 ? population.Average(p => p.Population) : 0;

    }
}