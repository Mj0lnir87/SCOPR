using MediatR;
using SCOPR.Application.Interfaces;
using SCOPR.Domain.Entities;
using SCOPR.Domain.Enums;
using System.Collections.Generic;

namespace SCOPR.Application.Queries.GetCountriesSummary;

public class GetCountriesSummaryQueryHandler : IRequestHandler<GetCountriesSummaryQuery, IList<CountrySummary>>
{
    private readonly ICountryRepository _countryRepository;
    private readonly IExchangeRateRepository _exchangeRateRepository;

    public GetCountriesSummaryQueryHandler(
        ICountryRepository countryRepository,
        IExchangeRateRepository exchangeRateRepository)
    {
        _countryRepository = countryRepository;
        _exchangeRateRepository = exchangeRateRepository;
    }

    /// <summary>
    /// Handles the GetCountriesSummaryQuery request.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IList<CountrySummary>> Handle(GetCountriesSummaryQuery request, CancellationToken cancellationToken)
    {
        var countries = await _countryRepository.GetAllAsync();

        IList<CountrySummary> countrySummaries = new List<CountrySummary>();

        foreach (var country in countries)
        {
            var exchangeRate = await _exchangeRateRepository.GetAverageRateInPeriodAsync(
                CurrencyCode.EUR.ToString(),
                country.Currency.Code,
                request.StartDate,
                request.EndDate);

            var population = await _countryRepository.GetAveragePopulationInPeriodAsync(
                country.Code.ToUpperInvariant(),
                request.StartDate,
                request.EndDate);

            countrySummaries.Add(CountrySummaryHelper.MapToCountrySummary(country, exchangeRate, population, request.StartDate, request.EndDate));
        }

        return countrySummaries;
    }
}