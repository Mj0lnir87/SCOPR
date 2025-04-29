using SCOPR.Domain.Entities;

namespace SCOPR.Application.Interfaces;

public interface ICountryRepository
{
    Task<Country> GetByCodeAsync(string code);
    Task<List<Country>> GetAllByCodesAsync(IList<string> codes);
    Task AddAsync(Country country);
    Task UpdateAsync(Country country);
    Task<CountrySummary> GetSummaryInPeriodAsync(string code, DateTime start, DateTime end);
}