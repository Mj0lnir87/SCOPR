using MediatR;
using SCOPR.Domain.Entities;
using System.Diagnostics.Metrics;
using Newtonsoft.Json.Linq;
using SCOPR.API.DTOs;
using SCOPR.Application.Interfaces;
using SCOPR.Domain.Enums;

namespace SCOPR.Application.Commands.FetchCountries;

public class FetchCountriesCommandHandler : IRequestHandler<FetchCountriesCommand, Unit>
{
    private readonly ICountryApiClient _countryApiClient;
    private readonly ICountryRepository _countryRepository;

    public FetchCountriesCommandHandler(
        ICountryApiClient countryApiClient,
        ICountryRepository countryRepository)
    {
        _countryApiClient = countryApiClient;
        _countryRepository = countryRepository;
    }

    public async Task<Unit> Handle(FetchCountriesCommand request, CancellationToken cancellationToken)
    {
        foreach (var countryCode in request.CountryCodes)
        {
            var countryDto = await _countryApiClient.GetCountryByCodeAsync(countryCode);

            // Get currency code from the DTO
            var currencyCode = countryDto.currencies.GetCurrencyCode();

            var country = MapToCountry(countryDto, currencyCode);

            var existingCountry = await _countryRepository.GetByCodeAsync(countryCode);
            if (existingCountry == null)
            {
                // Save new country to the database
                await _countryRepository.AddAsync(country);
            }
            else
            {
                // Update existing country with new information
                existingCountry.Name = country.Name;
                existingCountry.PhoneCodes = country.PhoneCodes;
                existingCountry.Capital = country.Capital;
                existingCountry.Population = country.Population;
                existingCountry.Currency = country.Currency;
                existingCountry.Flag = country.Flag;

                await _countryRepository.UpdateAsync(existingCountry);
            }
        }
        return Unit.Value;
    }

    private Country MapToCountry(CountryDto countryDto, string currencyCode)
    {
        var currencySymbol = countryDto.currencies.GetCurrencySymbol(currencyCode);
        var currencyName = countryDto.currencies.GetCurrencyName(currencyCode);

        IList<string> phoneCodes = new List<string>();
        foreach (var suffix in countryDto.idd.suffixes)
        {
            phoneCodes.Add($"{countryDto.idd.root}{suffix}");
        }

        // Validate the country DTO
        var country=  new Country
        {
            Code = countryDto.cioc,
            Name = countryDto.name.common,
            PhoneCodes = phoneCodes,
            Capital = countryDto.capital[0],
            Population = countryDto.population,
            Currency = new Currency
            {
                Code = currencyCode,
                Name = currencyName,
                Symbol = currencySymbol

            },
            Flag = countryDto.flag,
            CreatedAt = DateTime.Now
        };

        return country;
    }
}