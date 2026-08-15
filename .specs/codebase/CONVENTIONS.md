# Code Conventions

## Naming Conventions

**Files:** PascalCase matching the type they contain
Examples: `BeneficiarioController.cs`, `PlanoService.cs`, `AppDbContext.cs`

**Namespaces:** PascalCase, dotted, matching folder structure
Examples: `Api.Beneficiarios.Domain.Entities`, `Api.Beneficiarios.Application.DTOs.Beneficiario`

**Classes/Interfaces:** PascalCase, interfaces prefixed with `I`
Examples: `Beneficiario`, `IBeneficiarioRepository`, `PlanoController`

**Methods:** PascalCase, Portuguese names
Examples: `CriarBeneficiariosAsync()`, `ObterTodosBeneficiariosAsync()`, `ExcluirBeneficiarioSuavementeAsync()`

**Variables/Parameters:** camelCase, Portuguese names
Examples: `beneficiarioService`, `connectionString`, `dto`, `planoId`

**Private fields:** underscore-prefixed camelCase (`_camelCase`)
Example: `private readonly IBeneficiarioService _beneficiarioService;`

## Code Organization

**Using/Import ordering:** System namespaces first, then project namespaces (likely not formally enforced)

**File Structure (Controllers):**
1. Namespace declaration
2. Class with `[Route]`, `[ApiController]` attributes
3. Private readonly field for injected service
4. Constructor with DI
5. Action methods with HTTP verb attributes

**File Structure (Services):**
1. Namespace declaration
2. Class implementing interface
3. Constructor injecting repository(ies)
4. Public async methods for each operation

## Type Safety

**Approach:** Nullable reference types enabled project-wide (`<Nullable>enable</Nullable>`)
**Validation:** Data annotations on entities ([Required], [StringLength], [MaxLength])

## Error Handling

**Pattern:** try-catch in controllers for `InvalidOperationException` → `Conflict()` response. Null check on service results → `NotFound()` response.
**No global exception handler middleware.**
**No ProblemDetails RFC 7807 standard.**

## Comments/Documentation

**Style:** Minimal comments. XML docs not used. Swagger description in Program.cs for API metadata.

## Language

**Convention:** All identifiers, method names, and DTO property names in Portuguese (PT-BR).
**API routes:** English-style RESTful (`/api/Beneficiario`, `/api/Plano`).

## Async Patterns

**Convention:** All service and repository methods are async. `Async` suffix used consistently.
