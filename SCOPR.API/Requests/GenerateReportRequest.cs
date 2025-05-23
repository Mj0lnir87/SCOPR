﻿using SCOPR.Application.Interfaces;

namespace SCOPR.API.Requests
{
    public class GenerateReportRequest
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public IList<string> CountryCodes { get; set; }

        public GenerateReportRequest(DateTime startDate, DateTime endDate, IList<string> countryCodes)
        {
            StartDate = startDate;
            EndDate = endDate;
            CountryCodes = countryCodes;
        }
    }
}