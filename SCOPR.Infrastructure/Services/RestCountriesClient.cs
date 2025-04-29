using SCOPR.API.DTOs;
using SCOPR.Application.Interfaces;

namespace SCOPR.Infrastructure.Services;

public class RestCountriesClient : ICountryApiClient
{
    public Task<CountryDto> GetCountryByCodeAsync(string code)
    {
        throw new NotImplementedException();
    }
}