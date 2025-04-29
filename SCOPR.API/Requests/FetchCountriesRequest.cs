namespace SCOPR.API.Requests
{
    public class FetchCountriesRequest
    {
        public IList<string> CountryCodes { get; set; }

        public FetchCountriesRequest(List<string> countryCodes)
        {
            CountryCodes = countryCodes;
        }

    }
}
