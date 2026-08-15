# Codebase Concerns

## 1. Manual Validation Everywhere (Medium Risk)

**Evidence:** `BeneficiarioService.cs` — CPF validation is manual (length check, digit check, uniqueness query). Data annotations on entities but no validation pipeline.
**Impact:** Duplicated validation logic. Inconsistent error messages. Easy to miss validation in new features.
**Fix:** Adopt FluentValidation with a validation pipeline behavior via MediatR.

## 2. Try-Catch for Business Logic Errors (Medium Risk)

**Evidence:** `BeneficiarioController.cs:31,101`, `PlanoController.cs:30,103` — `catch (InvalidOperationException ex)` in controllers to return `Conflict()`.
**Impact:** Mixing HTTP concerns with business exceptions. No consistent error response format across the API.
**Fix:** Global exception handler middleware + Result pattern or custom exceptions mapped to ProblemDetails.

## 3. No Pagination on List Endpoints (Low Risk)

**Evidence:** `GET /api/Beneficiario` returns all results without pagination.
**Impact:** Will degrade performance as data grows. Inconsistent with production API standards.
**Fix:** Add pagination to all list endpoints with page/pageSize query params.

## 4. Open CORS Policy (High Risk for Production)

**Evidence:** `Program.cs` — `AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()`
**Impact:** Any domain can call the API. Security risk in production.
**Fix:** Restrict to specific origins per environment.

## 5. No API Versioning (Low Risk)

**Evidence:** Routes like `/api/Beneficiario` have no version prefix. Swagger only has v1 configured.
**Impact:** Breaking changes to API require careful coordination or introduce a new version.
**Fix:** Add URL-based versioning (`/api/v1/Beneficiario`) with Asp.Versioning.

## 6. Connection String via Plain Environment Variable (Low Risk)

**Evidence:** `DATABASE_CONNECTION_STRING` env var or plain `appsettings.json`.
**Impact:** Secrets visible in process environment. No secret manager integration.
**Fix:** Use .NET User Secrets in dev, Azure Key Vault / AWS Secrets Manager in production.

## 7. No Health Checks (Low Risk)

**Evidence:** No `AddHealthChecks()` in `Program.cs`.
**Impact:** No way for load balancers/orchestrators to check API health.
**Fix:** Add health check endpoint with database connectivity check.

## 8. Manual Entity-to-DTO Mapping (Low Risk)

**Evidence:** Services manually map properties between entities and DTOs.
**Impact:** Verbose, error-prone. New fields require mapping in multiple places.
**Fix:** Adopt AutoMapper with profiles.

## 9. Service Layer Coupling (Medium Risk)

**Evidence:** Controllers depend directly on `IBeneficiarioService`. Services mix reads and writes.
**Impact:** Hard to add cross-cutting concerns (logging, validation, caching) without touching every service. Hard to optimize reads vs writes separately.
**Fix:** Adopt CQRS with MediatR — separate Commands (writes) from Queries (reads).

## 10. No Integration Tests (Medium Risk)

**Evidence:** `tests/` directory only has unit tests with mocked repositories. No tests against real PostgreSQL.
**Impact:** Database queries, migrations, and EF Core configurations are untested.
**Fix:** Add integration tests with Testcontainers (PostgreSQL) or in-memory provider.
