using MediatR;
using SCOPR.Domain.Entities;

namespace SCOPR.Application.Commands.FetchCountries;

public class FetchCountriesCommand : IRequest<Unit>
{
    public IList<string> CountryCodes { get; }

    public FetchCountriesCommand(IList<string> countryCodes)
    {
        CountryCodes = countryCodes;
    }
}