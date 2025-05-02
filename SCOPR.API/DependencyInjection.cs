
using Microsoft.Extensions.DependencyInjection;
using SCOPR.Application.Commands.FetchCountries;
using SCOPR.Application.Interfaces;
using SCOPR.Infrastructure.Data;
using SCOPR.Infrastructure.Data.Repositories;
using SCOPR.Infrastructure.Reporting;
using SCOPR.Infrastructure.Services;
using MediatR;
using SCOPR.API.Controllers;
using SCOPR.Application.Queries.GetCountrySummary;
using System.Reflection;
using SCOPR.API.Requests;
using SCOPR.Application.Commands.GenerateReports;

namespace SCOPR.API;

public static class DependencyInjection
{
    /// <summary>
    /// Extension method to add infrastructure services to the service collection.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Add MediatR with the assembly containing your request handlers  
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // MediatR configuration  
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GenerateReportRequest).Assembly));
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GenerateReportCommand).Assembly));

        // MongoDB configuration
        services.Configure<MongoDbSettings>(configuration.GetSection("ConnectionStrings"));
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

    /// <summary>
    /// Configuration settings for MongoDB.
    /// </summary>
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
