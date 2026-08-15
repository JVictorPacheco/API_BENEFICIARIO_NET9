# Diário de Aprendizado — CQRS Migration

---

## T1 — Pacotes NuGet

**O que aprendi:** Dependências são a base. Sem pacotes, nada compila.
**Por que cada um:**
- `MediatR` — orquestra Commands/Queries, desacoplando controller da lógica
- `FluentValidation` — validação fora dos handlers, reaproveitável
- `AutoMapper` — evita mapeamento manual repetitivo (Entity ↔ DTO)
- `Serilog` — logging estruturado em JSON
- `Health Checks` + `API Versioning` — infra moderna de API

**Código aplicado:**
```xml
<!-- Application.csproj -->
<PackageReference Include="MediatR" Version="12.4.1" />
<PackageReference Include="FluentValidation" Version="11.11.0" />
<PackageReference Include="FluentValidation.DependencyInjectionExtensions" Version="11.11.0" />
<PackageReference Include="AutoMapper" Version="13.0.1" />

<!-- WebAPI.csproj -->
<PackageReference Include="MediatR" Version="12.4.1" />
<PackageReference Include="Serilog.AspNetCore" Version="8.0.3" />
<PackageReference Include="Asp.Versioning.Mvc" Version="8.1.0" />
<PackageReference Include="AspNetCore.HealthChecks.NpgSql" Version="8.0.2" />
```

---

## T2 — Exceções de Domínio

**O que aprendi:** Exceções específicas de domínio são o contrato de erro. Não é o controller que decide o status HTTP — é o tipo da exceção. Isso permite que o middleware capture tudo centralizado.

- `NotFoundException` → sempre vai retornar 404
- `BusinessRuleException` → sempre vai retornar 409
- `ValidationException` (Fluent) → sempre vai retornar 400
- Outras → 500

**Por que a NotFoundException tem mensagem fixa e a BusinessRuleException não?**
- "Não encontrado" sempre tem o mesmo formato: "X não encontrado" — então o construtor já monta
- Violação de regra pode ser qualquer coisa: CPF duplicado, plano com dependentes, etc. — mensagem livre

**Código aplicado:**
```csharp
// Domain/Exceptions/NotFoundException.cs
namespace Api.Beneficiarios.Domain.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string entityName, object key)
        : base($"'{entityName}' com id '{key}' não foi encontrado.") { }
}

// Domain/Exceptions/BusinessRuleException.cs
namespace Api.Beneficiarios.Domain.Exceptions;

public class BusinessRuleException : Exception
{
    public BusinessRuleException(string message) : base(message) { }
}
```

---

## T3 — DTO de Paginação

**O que aprendi:** Paginação é uma responsabilidade do backend. O frontend não deve calcular total de páginas, próxima página, etc. Esses metadados vêm prontos do servidor.

- `PaginatedResponse<T>` é genérico — serve pra qualquer lista (Beneficiário, Plano, etc.)
- Propriedades computadas (`TotalPages`, `HasNextPage`, `HasPreviousPage`) são lidas pelo frontend pra renderizar navegação
- Query string: `?page=2&pageSize=10`

**Como o frontend consome:**
```json
{
  "items": [ ... ],
  "page": 2,
  "pageSize": 10,
  "totalCount": 57,
  "totalPages": 6,
  "hasNextPage": true,
  "hasPreviousPage": true
}
```

**Código aplicado:**
```csharp
namespace Api.Beneficiarios.Application.DTOs.Common;

public class PaginatedResponse<T>
{
    public List<T> Items { get; set; } = [];
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}
```
