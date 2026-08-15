# Architecture

**Pattern:** Clean Architecture (4-layer)

## High-Level Structure

```
WebAPI (Controllers/Program.cs)
  ↓ depends on
Application (Services/DTOs)
  ↓ depends on
Domain (Entities/Enums/Interfaces)
  ↑ implemented by
Infrastructure (EF Core/Repositories/Configurations)
  ↑ registered via DI in WebAPI
```

## Identified Patterns

### Repository Pattern

**Location:** `src/Api.Beneficiarios.Domain/Interfaces/` + `src/Api.Beneficiarios.Infrastructure/Repositories/`
**Purpose:** Abstract data access behind interfaces defined in Domain
**Implementation:** Interfaces in Domain layer, EF Core implementations in Infrastructure
**Example:** `IBeneficiarioRepository.cs` → `BeneficiarioRepository.cs`

### Service Pattern

**Location:** `src/Api.Beneficiarios.Application/Services/`
**Purpose:** Business logic orchestration between controllers and repositories
**Implementation:** Services receive DTOs, perform validation manually, map to entities, call repositories
**Example:** `BeneficiarioService.cs` implements `IBeneficiarioService.cs`

### Base Entity Pattern

**Location:** `src/Api.Beneficiarios.Domain/Entities/BaseEntity.cs`
**Purpose:** Shared fields (Id, DataCadastro, DataAtualizacao, Excluido, DataExclusao)
**Implementation:** Abstract class with protected constructor, Guid.NewGuid() for Id

### Soft Delete Pattern

**Location:** `BaseEntity.cs` fields + service methods
**Purpose:** Logical exclusion instead of physical deletion
**Implementation:** `Excluido` flag + `DataExclusao` timestamp + `ExcluirSuavemente()` method

### DTO Pattern

**Location:** `src/Api.Beneficiarios.Application/DTOs/`
**Purpose:** Decouple API contracts from domain entities
**Implementation:** Separate Create/Update/Response DTOs per entity. Manual mapping in services.

## Data Flow

### Create Beneficiario

1. POST `/api/Beneficiario` with `CreateBeneficiarioDto`
2. Controller → `IBeneficiarioService.CriarBeneficiariosAsync(dto)`
3. Service validates CPF format (11 digits), checks uniqueness via repository
4. Service maps DTO → `Beneficiario` entity
5. Service calls `IBeneficiarioRepository.AddAsync(entity)` then `SaveChangesAsync()`
6. Service maps entity → `BeneficiarioResponseDto`
7. Returns `CreatedAtAction` with response DTO

### Read All / Filter

1. GET `/api/Beneficiario?status=Ativo&planoId=xxx`
2. Controller → `IBeneficiarioService.ObterTodosBeneficiariosAsync(status, planoId)`
3. Service calls repository with filter params
4. Maps entities → List of response DTOs
5. Returns `Ok(resultado)`

## Code Organization

**Approach:** Layer-based (Domain → Application → Infrastructure → WebAPI)

**Module boundaries:**
- Domain: zero external dependencies, only C# standard library
- Application: depends only on Domain
- Infrastructure: depends on Domain + Npgsql
- WebAPI: depends on all three, wires DI

## Current Architecture Gaps (target for v2)

- ❌ No CQRS — reads and writes use same service path
- ❌ No MediatR — tight coupling between controllers and services
- ❌ No pipeline behaviors — cross-cutting concerns inline
- ❌ Manual validation — no FluentValidation
- ❌ Manual mapping — no AutoMapper
- ❌ No Result pattern — try-catch in controllers for business errors
- ❌ No global exception handler middleware
- ❌ No pagination on list endpoints
