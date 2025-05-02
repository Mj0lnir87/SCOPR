using System.Net;
using Microsoft.Extensions.Configuration;
using SCOPR.Application.DTOs;
using SCOPR.Application.Interfaces;

namespace SCOPR.Infrastructure.Services;

public class RestCountriesClient : ICountryApiClient
{
    private static readonly string _section = "RestCountries";
    private static readonly string _url = "Url";
    private static readonly string _version =  "Version";

    private readonly string version;
    private readonly HttpClient _httpClient;

    public RestCountriesClient(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;        

        var section = config.GetSection(_section);
        version = section[_version];

        //Set base address for the HttpClient
        _httpClient.BaseAddress = new Uri(section[_url]);
    }

    /// <summary>
    /// Get a list of all countries.
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="KeyNotFoundException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="Exception"></exception>
    public async Task<CountryDto> GetCountryByCodeAsync(string code)
    {
        // Validate the input
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentNullException(nameof(code));
        }

        // Call the API to get the country details
        var response = await _httpClient.GetAsync($"{version}/alpha/{code}");

        if (!response.IsSuccessStatusCode)
        {
            switch (response.StatusCode)
            {
                case HttpStatusCode.NotFound:
                    throw new KeyNotFoundException($"Country with code {code} not found.");
                case HttpStatusCode.BadRequest:
                    throw new ArgumentException($"Invalid request for country code: {code}");
                case HttpStatusCode.InternalServerError:
                    throw new Exception("Internal server error while fetching country data.");
                default:
                    throw new Exception($"Unexpected error: {response.StatusCode}");
            }
        }
        // Deserialize the response to CountryDto usin Nwwtonsoft.Json
        var content = await response.Content.ReadAsStringAsync();
        var country = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CountryDto>>(content);
        if (country == null || !country.Any())
        {
            // Handle the case where no country is found
            throw new KeyNotFoundException($"Country with code {code} not found.");
        }

        // Return the first country found
        return country.First();
    }
}