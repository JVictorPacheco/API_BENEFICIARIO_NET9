# Testing Infrastructure

## Test Frameworks

**Unit:** xUnit 2.9.2 + Moq 4.20.72 + FluentAssertions 6.12.2
**Coverage:** coverlet.collector 6.0.2
**E2E:** Not configured

## Test Organization

**Location:** `tests/Api.Beneficiarios.Tests.Unit/`
**Naming:** `{ClassUnderTest}Tests.cs`
**Structure:** Mirrors source project structure (`Application/Services/`, `Domain/Validators/`, `Infrastructure/Repositories/`)

## Testing Patterns

### Unit Tests (Services)

**Approach:** Mock repository interface, instantiate service with mock, verify service logic and repository calls
**Location:** `tests/Api.Beneficiarios.Tests.Unit/Application/Services/`
**Pattern:** Constructor mock → Setup mock methods → Act → Assert with FluentAssertions

## Test Execution

**Commands:** `dotnet test` (from solution root or test project directory)

## Coverage Targets

**Current:** Service-layer tests only (`BeneficiarioServiceTests.cs`, `PlanoServiceTests.cs`)
**Gaps:** No repository integration tests, no controller tests, no domain validation tests
**Enforcement:** Not automated

## Test Coverage Matrix

| Code Layer | Required Test Type | Location Pattern | Run Command |
|-----------|-------------------|-----------------|-------------|
| Domain (Entities) | Unit | `tests/.../Domain/` | `dotnet test` |
| Application (Services) | Unit | `tests/.../Application/Services/` | `dotnet test` |
| Application (DTOs) | None (data-only) | N/A | N/A |
| Infrastructure (Repositories) | Integration | Not yet created | N/A |
| Infrastructure (DbContext) | Integration | Not yet created | N/A |
| WebAPI (Controllers) | Unit | Not yet created | N/A |

## Parallelism Assessment

| Test Type | Parallel-Safe? | Isolation Model | Evidence |
|-----------|---------------|-----------------|----------|
| Unit (Services) | Yes | Fully mocked dependencies | Moq mocks are per-test, no shared state |

## Gate Check Commands

| Gate Level | When to Use | Command |
|-----------|-------------|---------|
| Quick | After service/domain changes | `dotnet test tests/Api.Beneficiarios.Tests.Unit/` |
| Full | After infrastructure/migration changes | `dotnet test` (from solution root) |
| Build | After phase completion | `dotnet build && dotnet test` |
