using MediatR;
using SCOPR.API.DTOs;
using SCOPR.Application.Interfaces;
using SCOPR.Domain.Entities;
using SCOPR.Domain.Enums;
using System.Diagnostics.Metrics;

namespace SCOPR.Application.Queries.GetCountrySummary;

public class GetCountrySummaryQueryHandler : IRequestHandler<GetCountrySummaryQuery, CountrySummaryDto>
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

    public async Task<CountrySummaryDto> Handle(GetCountrySummaryQuery request, CancellationToken cancellationToken)
    {

        //get country data
        var countrySummary = _countryRepository.GetSummaryInPeriodAsync(request.CountryCode, request.StartDate, request.EndDate);

        if (countrySummary == null)
        {
            throw new ArgumentException($"No summary found for country code {request.CountryCode}");
        }

        CountrySummary country = countrySummary.Result;

        //calculate the average exchange rate for the country in the given period
        var exchangeRate = await _exchangeRateRepository.GetAverageRateInPeriodAsync(
            CurrencyCode.EUR.ToString(), 
            country.Currency.Code, 
            request.StartDate, 
            request.EndDate);

        return MapToDto(countrySummary.Result, exchangeRate, request.StartDate, request.EndDate);
    }

    //map the country summary to the DTO
    private CountrySummaryDto MapToDto(CountrySummary countrySummary, decimal exchangeRate, DateTime startDate, DateTime endDate)
    {
        return new CountrySummaryDto
        {
            CountryCode = countrySummary.CountryCode,
            CountryName = countrySummary.CountryName,
            PhoneCode = countrySummary.PhoneCode,
            Capital = countrySummary.Capital,
            Population = countrySummary.Population,
            Currency = new CurrencyDto
            {
                Code = countrySummary.Currency.Code,
                Name = countrySummary.Currency.Name,
                Symbol = countrySummary.Currency.Symbol
            },
            FlagUrl = countrySummary.FlagUrl,
            AverageExchangeRate = exchangeRate,
            StartDate = startDate,
            EndDate = endDate
        };
    }
}