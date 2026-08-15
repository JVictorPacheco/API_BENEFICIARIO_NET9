# External Integrations

## Database

**Service:** PostgreSQL
**Purpose:** Primary data store for Beneficiarios and Planos
**Implementation:** `src/Api.Beneficiarios.Infrastructure/Data/AppDbContext.cs` using EF Core with Npgsql provider
**Configuration:** Connection string via `DATABASE_CONNECTION_STRING` env var or `appsettings.json` → `ConnectionStrings:DefaultConnection`
**Authentication:** Username/password in connection string
**Migrations:** Code-first with EF Core Migrations (`src/Api.Beneficiarios.Infrastructure/Migrations/`)

## Containerization

**Service:** Docker + docker-compose
**Purpose:** Local development and production deployment
**Configuration:** `docker-compose.yml` (dev), `docker-compose.prod.yml` (production)
**Key files:** `Dockerfile`, `Dockerfile.dev`, `.dockerignore`, `.env`

## API Documentation

**Service:** Swagger UI (Swashbuckle)
**Purpose:** Interactive API documentation in development
**Configuration:** Root path (`/`), accessible at `http://localhost:5000/` in dev
**Version:** v1 (not versioned beyond this)

## No External APIs

The application is self-contained — no third-party API integrations, no webhooks, no message queues, no background jobs.
