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
        public string PhoneCode { get; set; }
        public string Capital { get; set; }
        public int Population { get; set; }
        public Currency Currency { get; set; }
        public string FlagUrl { get; set; }
        public decimal AverageExchangeRate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
