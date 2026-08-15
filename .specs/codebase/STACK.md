# Tech Stack

**Analyzed:** 2026-07-28

## Core

- Framework: .NET 9.0 (net9.0)
- Language: C# 13
- Runtime: ASP.NET Core 9.0
- Package manager: NuGet

## Backend

- API Style: REST (Controllers)
- ORM: Entity Framework Core 9.0.4
- Database: PostgreSQL (Npgsql.EntityFrameworkCore.PostgreSQL 9.0.4)
- Validation: Manual (data annotations + service-layer checks)
- Mapping: Manual (no AutoMapper)
- API Docs: Swagger (Swashbuckle.AspNetCore 6.5.0)
- CORS: Open (AllowAnyOrigin)

## Testing

- Unit: xUnit 2.9.2
- Mocking: Moq 4.20.72
- Assertions: FluentAssertions 6.12.2
- Coverage: coverlet.collector 6.0.2
- Test Runner: xunit.runner.visualstudio 2.8.2

## External Services

- Database: PostgreSQL (via Docker/docker-compose)

## Development Tools

- Containerization: Docker + docker-compose
- Build: dotnet CLI / Makefile
- Version control: Git (GitHub: JVictorPacheco/API_BENEFICIARIO_NET9)
- Environment: .env file for secrets
