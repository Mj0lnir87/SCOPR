# SCOPR - Test project

SCOPR is a comprehensive system for managing country information and exchange rates, designed with clean architecture and modern software development principles.

## Overview

SCOPR - Test project provides an API for retrieving, storing, and analyzing country data and exchange rates. It connects to external services like REST Countries API and Fixer.io, and generates reports on country economic data.

## Architecture

The project follows a Clean Architecture pattern with CQRS (Command Query Responsibility Segregation):

- **Domain Layer**: Contains core business entities and rules
- **Application Layer**: Contains business use cases and interfaces
- **Infrastructure Layer**: Contains implementations of interfaces and external services
- **API Layer**: Presents HTTP endpoints for clients

## Features

- Fetch and store country information from REST Countries API
- Retrieve and monitor exchange rates between different currencies
- Generate country data reports with current exchange rates
- RESTful API endpoints for all functionality

## Getting Started

### Prerequisites

- .NET 8.0 or higher
- MongoDB (for data storage)
- API keys for exchange rate providers (e.g., Fixer.io)

### Installation

1. Clone the repository:
   ```
   git clone https://github.com/Mj0lnir87/SCOPR.git
   cd SCOPR
   ```

2. Configure settings in `appsettings.json`:
   ```json
   {
      "Logging": {
        "LogLevel": {
        "Default": "Information",
        "Microsoft.AspNetCore": "Warning"
        }
    },
    "AllowedHosts": "*",
    "ConnectionStrings": {
       "ConnectionString": "mongodb://localhost:27017",
       "DatabaseName": "scopr"
     },
    "RestCountries": {
        "Url": "https://restcountries.com/",
        "Version": "3.1"
    },
    "Fixer": {
        "Url": "https://api.fixer.io/api/",
        "ApiKey": "your-api-key-here"
    }
   }
   ```

3. Build and run the application:
   ```
   dotnet build
   cd SCOPR.API
   dotnet run
   ```

4. The API will be available at `https://localhost:7011` by default

## API Endpoints

### Countries

- `GET /api/countries/summary/{code}` - Get country summary by country code
- `GET /api/countries/summary` - Get all countries summary
- `POST /api/countries/fetch` - Refresh country data from external API

### Exchange Rates

- `POST /api/exchangerates/fetch` - Refresh exchange rates from external API

### Reports

- `POST /api/reports/countries/summary` - Generate country report with current exchange rates

## Project Structure

```
SCOPR/
├── SCOPR.API                 # API Controllers and configuration
├── SCOPR.Application         # Use cases, commands, queries, and interfaces
├── SCOPR.Domain              # Core entities and business rules
├── SCOPR.Infrastructure      # Implementation of interfaces

```

## Technologies

- **ASP.NET Core**: Web API framework
- **MediatR**: CQRS implementation
- **MongoDB**: Document database
- **QuestPDF**: PDF report generation

## Development

### Adding New Features

1. Add domain entities to SCOPR.Domain if needed
2. Add commands/queries to SCOPR.Application
3. Implement any required services in SCOPR.Infrastructure
4. Add API endpoints in SCOPR.API

## Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgments

- [REST Countries API](https://restcountries.com) for country data
- [Fixer.io](https://fixer.io) for exchange rate data

## Contact

Project Link: [https://github.com/Mj0lnir87/SCOPR](https://github.com/Mj0lnir87/SCOPR)