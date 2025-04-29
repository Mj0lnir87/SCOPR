using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SCOPR.Application.Interfaces;
using SCOPR.Infrastructure.Data;
using SCOPR.Infrastructure.Data.Repositories;
using SCOPR.Infrastructure.Reporting;
using SCOPR.Infrastructure.Services;

namespace SCOPR.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // MongoDB configuratie
        services.Configure<MongoDbSettings>(configuration.GetSection("MongoDbSettings"));
        services.AddSingleton<DbContext>();

        // Repositories
        services.AddScoped<ICountryRepository, CountryRepo>();
        services.AddScoped<IExchangeRateRepository, ExchangeRateRepo>();

        // HTTP clients
        services.AddHttpClient<ICountryApiClient, RestCountriesClient>();
        services.AddHttpClient<IExchangeRateApiClient, FixerApiClient>();

        // Rapport generator
        services.AddScoped<IReportGenerator, QuestPdfReportGenerator>();

        return services;
    }
}

public class MongoDbSettings
{
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
}