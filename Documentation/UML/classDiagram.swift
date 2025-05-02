---
config:
  layout: dagre
---
classDiagram
direction TB
    class CountriesController {
	    -IMediator mediator
	    +FetchCountriesAsync(FetchCountriesRequest) Task~IActionResult~
	    +GetCountryAsync(string countryCode, DateTime startDate, DateTime endDate) Task~ActionResult~CountrySummary~~
		+GetCountriesAsync([FromQuery]IList<string> countryCodes, DateTime startDate, DateTime endDate) Task~ActionResult~IList~CountrySummary~~~
    }
    class ExchangeRatesController {
	    -IMediator mediator
	    +FetchExchangeRatesAsync(FetchExchangeRatesRequest) Task~IActionResult~
    }
    class ReportsController {
	    -IMediator mediator
	    +GenerateCountrySummaryReportAsync(GenerateReportRequest) Task~IActionResult~
    }
    class FetchCountriesRequest {
	    +IList~string~ CountryCodes
	    +FetchCountriesRequest(IList~string~)
    }
    class FetchExchangeRatesRequest {
	    +string BaseCurrency
	    +List~string~ TargetCurrencies
	    +DateTime StartDate
	    +DateTime EndDate
	    +FetchExchangeRatesRequest(string, List~string~, DateTime, DateTime)
    }
    class GenerateReportRequest {
	    +DateTime StartDate
	    +DateTime EndDate
	    +IList~string~ CountryCodes
	    +GenerateReportRequest(DateTime, DateTime, IList~string~)
    }
    class FetchCountriesCommand {
	    +IList~string~ CountryCodes
	    +IList~string~ CountryCodes
	    +FetchCountriesCommand(IList~string~)
	    +FetchCountriesCommand(IList~string~)
    }
    class FetchCountriesCommandHandler {
	    -ICountryApiClient _countryApiClient
	    -ICountryRepository _countryRepository
	    -ICountryApiClient _countryApiClient
	    -ICountryRepository _countryRepository
	    +Handle(FetchCountriesCommand, CancellationToken) Task~Unit~
	    -MapToCountry(CountryDto, string) Country
	    +Task~Unit~ Handle(FetchCountriesCommand, CancellationToken)
	    -Country MapToCountry(CountryDto, string)
    }
    class FetchExchangeRatesCommand {
	    +string BaseCurrency
	    +List~string~ TargetCurrencies
	    +DateTime StartDate
	    +DateTime EndDate
	    +string BaseCurrency
	    +List~string~ TargetCurrencies
	    +DateTime StartDate
	    +DateTime EndDate
	    +FetchExchangeRatesCommand(string, List~string~, DateTime, DateTime)
	    +FetchExchangeRatesCommand(string, List~string~, DateTime, DateTime)
    }
    class FetchExchangeRatesCommandHandler {
	    -IExchangeRateApiClient _exchangeRateApiClient
	    -IExchangeRateRepository _exchangeRateRepository
	    -IExchangeRateApiClient _exchangeRateApiClient
	    -IExchangeRateRepository _exchangeRateRepository
	    +Handle(FetchExchangeRatesCommand, CancellationToken) Task~Unit~
	    +Task~Unit~ Handle(FetchExchangeRatesCommand, CancellationToken)
    }
    class GenerateReportCommand {
	    +DateTime StartDate
	    +DateTime EndDate
	    +IList~string~ CountryCodes
	    +DateTime StartDate
	    +DateTime EndDate
	    +IList~string~ CountryCodes
	    +GenerateReportCommand(DateTime, DateTime, IList~string~)
	    +GenerateReportCommand(DateTime, DateTime, IList~string~)
    }
    class GenerateReportCommandHandler {
	    -IReportGenerator _reportGenerator
	    -IExchangeRateRepository _exchangeRateRepository
	    -ICountryRepository _countryRepository
	    -IMediator mediator
	    -IReportGenerator _reportGenerator
	    -IExchangeRateRepository _exchangeRateRepository
	    -ICountryRepository _countryRepository
	    -IMediator mediator
	    +Handle(GenerateReportCommand, CancellationToken) Task~byte[]~
	    +Task~byte[]~ Handle(GenerateReportCommand, CancellationToken)
    }
    class GetCountrySummaryQuery {
	    +string CountryCode
	    +DateTime StartDate
	    +DateTime EndDate
	    +string CountryCode
	    +DateTime StartDate
	    +DateTime EndDate
	    +GetCountrySummaryQuery(string, DateTime, DateTime)
    }
    class GetCountrySummaryQueryHandler {
	    -ICountryRepository _countryRepository
	    -IExchangeRateRepository _exchangeRateRepository
	    -ICountryRepository _countryRepository
	    -IExchangeRateRepository _exchangeRateRepository
	    +Handle(GetCountrySummaryQuery, CancellationToken) Task~CountrySummary~
	    -MapToCountrySummary(Country, decimal, double, DateTime, DateTime) CountrySummary
	    +Task~CountrySummary~ Handle(GetCountrySummaryQuery, CancellationToken)
	    -CountrySummary MapToCountrySummary(Country, decimal, double, DateTime, DateTime)
    }
	class GetCountriesSummaryQuery {
	    +string CountryCode
	    +DateTime StartDate
	    +DateTime EndDate
	    +string CountryCode
	    +DateTime StartDate
	    +DateTime EndDate
	    +GetCountriesSummaryQuery(string, DateTime, DateTime)
    }
    class GetCountriesSummaryQueryHandler {
	    -ICountryRepository _countryRepository
	    -IExchangeRateRepository _exchangeRateRepository
	    -ICountryRepository _countryRepository
	    -IExchangeRateRepository _exchangeRateRepository
	    +Handle(GetCountrySummaryQuery, CancellationToken) Task~CountrySummary~
	    -MapToCountrySummary(Country, decimal, double, DateTime, DateTime) CountrySummary
	    +Task~CountrySummary~ Handle(GetCountrySummaryQuery, CancellationToken)
	    -CountrySummary MapToCountrySummary(Country, decimal, double, DateTime, DateTime)
    }
    class IMediator {
	    +Send(object) Task~object~
    }
    class Country {
	    +string Id
	    +string Code
	    +string Name
	    +IList~string~ PhoneCodes
	    +string Capital
	    +int Population
	    +Currency Currency
	    +string Flag
	    +DateTime CreatedAt
    }
    class Currency {
	    +string Code
	    +string Name
	    +string Symbol
    }
    class ExchangeRate {
	    +string Id
	    +string BaseCurrencyCode
	    +string TargetCurrencyCode
	    +DateTime Date
	    +decimal Rate
	    +DateTime CreatedAt
	    +DateTime UpdatedAt
    }
    class CountrySummary {
	    +string CountryCode
	    +string CountryName
	    +IList~string~ PhoneCodes
	    +string Capital
	    +double AveragePopulation
	    +Currency Currency
	    +string FlagUrl
	    +decimal AverageExchangeRate
	    +DateTime StartDate
	    +DateTime EndDate
    }
    class CurrencyCode {
	    EUR
	    USD
	    GBP
	    AUD
	    BRL
	    CNY
    }
    class CountryDto {
	    +Name name
	    +string cioc
	    +Currencies currencies
	    +Idd idd
	    +List~string~ capital
	    +string flag
	    +int population
    }
    class ExchangeRateDto {
	    +bool success
	    +int timestamp
	    +string base
	    +DateTime date
	    +Rates rates
	    +Error error
    }
    class Name {
	    +string common
	    +string official
    }
    class Currencies {
	    +Dictionary~string,JToken~ Items
	    +JToken this[string currencyCode]
	    +IEnumerable~string~ AvailableCurrencies
	    +bool HasCurrency(string)
	    +string GetCurrencyCode()
	    +string GetCurrencySymbol(string)
	    +string GetCurrencyName(string)
    }
    class Idd {
	    +string root
	    +List~string~ suffixes
    }
    class Rates {
	    +Dictionary~string,JToken~ Items
	    +JToken this[string currencyCode]
    }
    class Error {
	    +int code
	    +string type
	    +string info
    }
    class ICountryRepository {
	    +Task~Country~ GetByCodeAsync(string)
		+Task~IList~Country~~ GetAllAsync()
	    +Task AddAsync(Country)
	    +Task UpdateAsync(Country)
	    +Task~double~ GetAveragePopulationInPeriodAsync(string, DateTime, DateTime)
    }
    class IExchangeRateRepository {
	    +Task~ExchangeRate~ GetLatestRateAsync(string, string, DateTime)
	    +Task~decimal~ GetAverageRateInPeriodAsync(string, string, DateTime, DateTime)
	    +Task AddAsync(ExchangeRate)
	    +Task UpdateAsync(ExchangeRate)
    }
    class ICountryApiClient {
	    +Task~CountryDto~ GetCountryByCodeAsync(string)
    }
    class IExchangeRateApiClient {
	    +Task~IList~ExchangeRateDto~~ GetLatestRatesAsync(string, List~string~, DateTime, DateTime)
    }
    class IReportGenerator {
	    +Task~byte[]~ GenerateCountrySummaryReportAsync(List~CountrySummary~)
    }
    class CountryRepo {
	    -DbContext _dbContext
	    +Task AddAsync(Country)
	    +Task~Country~ GetByCodeAsync(string)
	    +Task UpdateAsync(Country)
	    +Task~double~ GetAveragePopulationInPeriodAsync(string, DateTime, DateTime)
    }
    class ExchangeRateRepo {
	    -DbContext _dbContext
	    +Task~ExchangeRate~ GetLatestRateAsync(string, string, DateTime)
	    +Task~decimal~ GetAverageRateInPeriodAsync(string, string, DateTime, DateTime)
	    +Task AddAsync(ExchangeRate)
	    +Task UpdateAsync(ExchangeRate)
    }
    class RestCountriesClient {
	    -string version
	    -HttpClient _httpClient
	    +Task~CountryDto~ GetCountryByCodeAsync(string)
    }
    class FixerApiClient {
	    -string apiKey
	    -HttpClient _httpClient
	    +Task~IList~ExchangeRateDto~~ GetLatestRatesAsync(string, List~string~, DateTime, DateTime)
    }
    class QuestPdfReportGenerator {
	    +Task~byte[]~ GenerateCountrySummaryReportAsync(List~CountrySummary~)
	    -void RenderContent(IContainer, List~CountrySummary~)
    }
    class DbContext {
	    -IMongoDatabase _db
	    +DbContext(IConfiguration)
	    +IMongoCollection~T~ GetCollection~T~(string)
	    -void ValidateConnectionsettings(string, string)
    }

	<<interface>> IMediator
	<<enumeration>> CurrencyCode
	<<interface>> ICountryRepository
	<<interface>> IExchangeRateRepository
	<<interface>> ICountryApiClient
	<<interface>> IExchangeRateApiClient
	<<interface>> IReportGenerator

    CountriesController --> IMediator
    ExchangeRatesController --> IMediator
    ReportsController --> IMediator
    CountriesController ..> FetchCountriesRequest
    CountriesController ..> FetchCountriesCommand
    CountriesController ..> GetCountrySummaryQuery
    CountriesController ..> GetCountriesSummaryQuery
    ExchangeRatesController ..> FetchExchangeRatesRequest
    ExchangeRatesController ..> FetchExchangeRatesCommand
    ReportsController ..> GenerateReportRequest
    ReportsController ..> GenerateReportCommand
    Country "1" *-- "1" Currency
    CountrySummary "1" *-- "1" Currency
    CountryDto "1" *-- "1" Name
    CountryDto "1" *-- "1" Currencies
    CountryDto "1" *-- "1" Idd
    ExchangeRateDto "1" *-- "1" Rates
    ExchangeRateDto "1" *-- "0..1" Error
    CountryRepo ..|> ICountryRepository
    ExchangeRateRepo ..|> IExchangeRateRepository
    RestCountriesClient ..|> ICountryApiClient
    FixerApiClient ..|> IExchangeRateApiClient
    QuestPdfReportGenerator ..|> IReportGenerator
    FetchCountriesCommandHandler --> ICountryApiClient
    FetchCountriesCommandHandler --> ICountryRepository
    FetchExchangeRatesCommandHandler --> IExchangeRateApiClient
    FetchExchangeRatesCommandHandler --> IExchangeRateRepository
    GenerateReportCommandHandler --> IReportGenerator
    GenerateReportCommandHandler --> ICountryRepository
    GenerateReportCommandHandler --> IExchangeRateRepository
    GetCountrySummaryQueryHandler --> ICountryRepository
    GetCountrySummaryQueryHandler --> IExchangeRateRepository    
	GetCountriesSummaryQueryHandler --> ICountryRepository
    GetCountriesSummaryQueryHandler --> IExchangeRateRepository
    CountryRepo --> DbContext
    ExchangeRateRepo --> DbContext

	class CountryDto:::Peach
	class ExchangeRateDto:::Peach
	class Name:::Peach
	class Currencies:::Peach
	class Idd:::Peach
	class Rates:::Peach
	class Error:::Peach

	classDef Peach :,stroke-width:1px, stroke-dasharray:none, stroke:#FBB35A, fill:#FFEFDB, color:#8F632D
