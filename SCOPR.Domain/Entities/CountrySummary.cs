using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCOPR.Domain.Entities
{
    public class CountrySummary
    {
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public IList<string> PhoneCodes { get; set; }
        public string Capital { get; set; }
        public double AveragePopulation { get; set; }
        public Currency Currency { get; set; }
        public string Flag { get; set; }
        public decimal AverageExchangeRate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
