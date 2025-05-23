﻿using SCOPR.Domain.Entities;

namespace SCOPR.Application.Interfaces;

public interface ICountryRepository
{
    Task<Country> GetByCodeAsync(string code);
    Task<IList<Country>> GetAllAsync();
    Task AddAsync(Country country);
    Task UpdateAsync(Country country);
    Task<double> GetAveragePopulationInPeriodAsync(string requestCountryCode, DateTime requestStartDate, DateTime requestEndDate);
}