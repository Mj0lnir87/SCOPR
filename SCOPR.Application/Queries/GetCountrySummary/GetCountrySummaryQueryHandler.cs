using MediatR;
using SCOPR.Application.DTOs;
using SCOPR.Application.Interfaces;
using SCOPR.Domain.Entities;
using SCOPR.Domain.Enums;

namespace SCOPR.Application.Queries.GetCountrySummary;

public class GetCountrySummaryQueryHandler : IRequestHandler<GetCountrySummaryQuery, CountrySummary>
{
    private readonly ICountryRepository _countryRepository;
    private readonly IExchangeRateRepository _exchangeRateRepository;
    public GetCountrySummaryQueryHandler(
        ICountryRepository countryRepository,
        IExchangeRateRepository exchangeRateRepository)
    {
        _countryRepository = countryRepository;
        _exchangeRateRepository = exchangeRateRepository;
    }

    /// <summary>
    /// Handles the GetCountrySummaryQuery request.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    public async Task<CountrySummary> Handle(GetCountrySummaryQuery request, CancellationToken cancellationToken)
    {
        // Get country data
        var country = await _countryRepository.GetByCodeAsync(request.CountryCode.ToUpperInvariant());

        if (country == null)
        {
            throw new KeyNotFoundException($"No country found for country code {request.CountryCode}");
        }

        // Get the average exchange rate for the country in the given period
        var exchangeRate = await _exchangeRateRepository.GetAverageRateInPeriodAsync(
            CurrencyCode.EUR.ToString(), 
            country.Currency.Code, 
            request.StartDate, 
            request.EndDate);

        // Get the average population for the country in the given period
        var population = await _countryRepository.GetAveragePopulationInPeriodAsync(
            request.CountryCode.ToUpperInvariant(),
            request.StartDate,
            request.EndDate);

        return MapToCountrySummary(country, exchangeRate, population, request.StartDate, request.EndDate);
    }

    /// <summary>
    /// Maps the country data to a CountrySummary object.
    /// </summary>
    /// <param name="country"></param>
    /// <param name="exchangeRate"></param>
    /// <param name="population"></param>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <returns></returns>
    private CountrySummary MapToCountrySummary(Country country, decimal exchangeRate, double population, DateTime startDate, DateTime endDate)
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