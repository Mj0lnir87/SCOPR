using MediatR;
using SCOPR.API.DTOs;

namespace SCOPR.Application.Queries.GetCountrySummary;

public class GetCountrySummaryQuery : IRequest<CountrySummaryDto>
{
    public string CountryCode { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public GetCountrySummaryQuery(string countryCode, DateTime startDate, DateTime endDate)
    {
        CountryCode = countryCode;
        StartDate = startDate;
        EndDate = endDate;
    }
}