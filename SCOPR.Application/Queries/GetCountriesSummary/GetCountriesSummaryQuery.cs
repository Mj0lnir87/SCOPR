using MediatR;
using SCOPR.Domain.Entities;

namespace SCOPR.Application.Queries.GetCountriesSummary;

public class GetCountriesSummaryQuery : IRequest<IList<CountrySummary>>
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public GetCountriesSummaryQuery(DateTime startDate, DateTime endDate)
    {
        StartDate = startDate;
        EndDate = endDate;
    }
}