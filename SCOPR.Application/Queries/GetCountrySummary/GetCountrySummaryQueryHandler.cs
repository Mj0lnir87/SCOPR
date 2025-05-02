using MediatR;
using SCOPR.API.DTOs;
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

    // Map the country summary to the DTO
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
            FlagUrl = country.Flag,
            AverageExchangeRate = exchangeRate,
            StartDate = startDate,
            EndDate = endDate
        };
    }
}