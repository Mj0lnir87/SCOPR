using MediatR;
using SCOPR.Domain.Entities;
using System.Diagnostics.Metrics;
using SCOPR.Application.Interfaces;

namespace SCOPR.Application.Commands.FetchCountries;

public class FetchCountriesCommandHandler : IRequestHandler<FetchCountriesCommand>
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
    public async Task Handle(FetchCountriesCommand request, CancellationToken cancellationToken)
    {
        foreach (var countryCode in request.CountryCodes)
        {
            var countryDto = await _countryApiClient.GetCountryByCodeAsync(countryCode);

            // Validate the country DTO
            var country = new Country
            {
                Code = countryDto.Code,
                Name = countryDto.Name,
                PhoneCode = countryDto.PhoneCode,
                Capital = countryDto.Capital,
                Population = countryDto.Population,
                Currency = new Currency(countryDto.Currency.Code, countryDto.Currency.Name, countryDto.Currency.Symbol),
                FlagUrl = countryDto.FlagUrl,
                CreatedAt = DateTime.UtcNow
            };

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
                existingCountry.PhoneCode = country.PhoneCode;
                existingCountry.Capital = country.Capital;
                existingCountry.Population = country.Population;
                existingCountry.Currency = country.Currency;
                existingCountry.FlagUrl = country.FlagUrl;

                await _countryRepository.UpdateAsync(existingCountry);
            }
        }
    }
}