using SCOPR.Domain.Entities;

namespace SCOPR.Application.Queries;

public static class CountrySummaryHelper
{

    /// <summary>
    /// Maps the country data to a CountrySummary object.
    /// </summary>
    /// <param name="country"></param>
    /// <param name="exchangeRate"></param>
    /// <param name="population"></param>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <returns></returns>
    public static CountrySummary MapToCountrySummary(Country country, decimal exchangeRate, double population, DateTime startDate, DateTime endDate)
    {
        return new CountrySummary
        {
            CountryCode = country.Code,
            CountryName = country.Name,
            PhoneCodes = country.PhoneCodes,
            Capital = country.Capital,
            AveragePopulation = population,
            Currency = new Currency
            {
                Code = country.Currency.Code,
                Name = country.Currency.Name,
                Symbol = country.Currency.Symbol
            },
            Flag = country.Flag,
            AverageExchangeRate = exchangeRate,
            StartDate = startDate,
            EndDate = endDate
        };
    }
}