using MediatR;
using SCOPR.API.DTOs;
using SCOPR.Application.Interfaces;
using SCOPR.Application.Queries.GetCountrySummary;
using SCOPR.Domain.Entities;
using SCOPR.Domain.Enums;

namespace SCOPR.Application.Commands.GenerateReports;

public class GenerateReportCommandHandler : IRequestHandler<GenerateReportCommand, byte[]>
{
    private readonly IReportGenerator _reportGenerator;
    private readonly IExchangeRateRepository _exchangeRateRepository;
    private readonly ICountryRepository _countryRepository;
    private readonly IMediator mediator;

    public GenerateReportCommandHandler(
        IReportGenerator reportGenerator,
        IExchangeRateRepository exchangeRateRepository,
        ICountryRepository countryRepository,
        IMediator mediator)
    {
        _reportGenerator = reportGenerator;
        _exchangeRateRepository = exchangeRateRepository;
        _countryRepository = countryRepository;
        this.mediator = mediator;
    }

    public async Task<byte[]> Handle(GenerateReportCommand request, CancellationToken cancellationToken)
    {
        List<CountrySummary> countrySummaries = new List<CountrySummary>();
        foreach (var countryCode in request.CountryCodes)
        {
            var country = await mediator.Send(new GetCountrySummaryQuery(countryCode, request.StartDate, request.EndDate));
            if (country == null)
            {
                throw new KeyNotFoundException($"No country found for country code {countryCode}");
            }

            countrySummaries.Add(country);
        }

        return await _reportGenerator.GenerateCountrySummaryReportAsync(countrySummaries);
    }
}