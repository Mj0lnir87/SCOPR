using SCOPR.Application.DTOs;

namespace SCOPR.Application.Interfaces;

public interface ICountryApiClient
{
    Task<CountryDto> GetCountryByCodeAsync(string code);
}