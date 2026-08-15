# Backend CQRS Migration — Tasks

**Design:** `.specs/features/backend-cqrs-migration/design.md`
**Status:** Draft

---

## Execution Plan

```
Phase 1 (Foundation):    T1 → T2 → T3 → T4 → T5 → T6 → T7
Phase 2 (Beneficiarios): T7 → T8 → T9 → T10 → T11
Phase 3 (Planos):        T11 → T12 → T13 → T14 → T15
Phase 4 (Clean-up):      T15 → T16 → T17
```

---

## Phase 1: Foundation (Sequencial)

### T1: Adicionar pacotes NuGet

**What**: Adicionar MediatR, FluentValidation, AutoMapper, Serilog, HealthChecks, API Versioning nos `.csproj`
**Where**:
- `src/Api.Beneficiarios.Application/Api.Beneficiarios.Application.csproj` (MediatR, FluentValidation, AutoMapper)
- `src/Api.Beneficiarios.WebAPI/Api.Beneficiarios.WebAPI.csproj` (MediatR, Serilog, Asp.Versioning.Mvc, HealthChecks.NpgSql)
**Depends on**: None
**Reuses**: N/A
**Requirement**: BEM-01, BEM-03, BEM-04, BEM-05, BEM-06

**Packages to add:**
```xml
<!-- Application.csproj -->
<PackageReference Include="MediatR" Version="12.4.1" />
<PackageReference Include="FluentValidation" Version="11.11.0" />
<PackageReference Include="FluentValidation.DependencyInjectionExtensions" Version="11.11.0" />
<PackageReference Include="AutoMapper" Version="13.0.1" />

<!-- WebAPI.csproj (mantendo o que já tem +) -->
<PackageReference Include="MediatR" Version="12.4.1" />
<PackageReference Include="Serilog.AspNetCore" Version="8.0.3" />
<PackageReference Include="Asp.Versioning.Mvc" Version="8.1.0" />
<PackageReference Include="AspNetCore.HealthChecks.NpgSql" Version="8.0.2" />
```

**Done when**:
- [ ] `dotnet restore` executa sem erros
- [ ] `dotnet build` compila sem erros (ainda com código antigo)

**Tests**: none (infra change only)
**Gate**: `dotnet build`
**Commit**: `chore: adicionar pacotes MediatR, FluentValidation, AutoMapper, Serilog`

---

### T2: Criar exceções de domínio

**What**: Criar `NotFoundException` e `BusinessRuleException` no Domain
**Where**: `src/Api.Beneficiarios.Domain/Exceptions/NotFoundException.cs`, `BusinessRuleException.cs`
**Depends on**: T1
**Reuses**: N/A
**Requirement**: BEM-05

**Done when**:
- [ ] `NotFoundException` com construtor `(string entityName, object key)`
- [ ] `BusinessRuleException` com construtor `(string message)`
- [ ] `dotnet build` compila

**Tests**: none (data-only classes)
**Gate**: `dotnet build`
**Commit**: `feat: adicionar exceções de domínio (NotFound, BusinessRule)`

---

### T3: Criar DTO de paginação

**What**: Criar `PaginatedResponse<T>` na pasta de DTOs
**Where**: `src/Api.Beneficiarios.Application/DTOs/Common/PaginatedResponse.cs`
**Depends on**: T1
**Reuses**: N/A
**Requirement**: BEM-06

**Formato:**
```csharp
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

**Done when**:
- [ ] Classe criada com todas as propriedades
- [ ] `dotnet build` compila

**Tests**: none (data-only class)
**Gate**: `dotnet build`
**Commit**: `feat: adicionar DTO de paginação`

---

### T4: Criar AutoMapper Profiles

**What**: Criar Profiles do AutoMapper — mapeamento Entity ↔ DTO
**Where**:
- `src/Api.Beneficiarios.Application/Mappings/BeneficiarioProfile.cs`
- `src/Api.Beneficiarios.Application/Mappings/PlanoProfile.cs`
**Depends on**: T2
**Reuses**: DTOs existentes (`BeneficiarioResponseDto`, `CreateBeneficiarioDto`, etc.)
**Requirement**: BEM-04

**Mapeamentos:**
```csharp
// BeneficiarioProfile
CreateMap<Beneficiario, BeneficiarioResponseDto>()
    .ForMember(dest => dest.NomePlano, opt => opt.MapFrom(src => src.Plano.NomePlano));

CreateMap<CreateBeneficiarioDto, Beneficiario>();
CreateMap<UpdateBeneficiarioDto, Beneficiario>()
    .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

// PlanoProfile
CreateMap<Plano, PlanoResponseDto>();
CreateMap<CreatePlanoDto, Plano>();
CreateMap<UpdatePlanoDto, Plano>()
    .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
```

**Done when**:
- [ ] Ambos Profiles criados com mapeamentos corretos
- [ ] `dotnet build` compila

**Tests**: none (configuração do AutoMapper, testado indiretamente via handlers)
**Gate**: `dotnet build`
**Commit**: `feat: adicionar AutoMapper Profiles para Beneficiario e Plano`

---

### T5: Criar ValidationBehavior (Pipeline do MediatR)

**What**: Criar o pipeline behavior que executa FluentValidation automaticamente antes de cada handler
**Where**: `src/Api.Beneficiarios.Application/Behaviors/ValidationBehavior.cs`
**Depends on**: T2
**Reuses**: N/A
**Requirement**: BEM-03

```csharp
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        if (!_validators.Any()) return await next();

        var context = new ValidationContext<TRequest>(request);
        var failures = _validators
            .Select(v => v.Validate(context))
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count != 0)
            throw new ValidationException(failures);

        return await next();
    }
}
```

**Done when**:
- [ ] Behavior criado implementando `IPipelineBehavior<TRequest, TResponse>`
- [ ] Lança `ValidationException` quando há erros
- [ ] Chama `next()` quando não há erros
- [ ] `dotnet build` compila

**Tests**: none (comportamento do MediatR, testado via testes de integração dos handlers)
**Gate**: `dotnet build`
**Commit**: `feat: adicionar ValidationBehavior (pipeline MediatR)`

---

### T6: Criar GlobalExceptionMiddleware

**What**: Criar middleware que captura exceções e retorna ProblemDetails (RFC 7807)
**Where**: `src/Api.Beneficiarios.WebAPI/Middleware/GlobalExceptionMiddleware.cs`
**Depends on**: T2
**Reuses**: Exceções de domínio criadas em T2
**Requirement**: BEM-05

**Mapeamento de exceções → HTTP:**
- `NotFoundException` → 404
- `ValidationException` (FluentValidation) → 400
- `BusinessRuleException` → 409
- Qualquer outra → 500

**Done when**:
- [ ] Middleware criado com `InvokeAsync(HttpContext, RequestDelegate)`
- [ ] Mapeia cada tipo de exceção pro HTTP status correto
- [ ] Retorna `ProblemDetails` com `Title`, `Status`, `Detail`, `Extensions["errors"]` (para validação)
- [ ] Não expõe stack trace em produção (usa `IWebHostEnvironment`)
- [ ] `dotnet build` compila

**Tests**: none (testado via integração — T17)
**Gate**: `dotnet build`
**Commit**: `feat: adicionar GlobalExceptionMiddleware com ProblemDetails`

---

### T7: Atualizar Program.cs (DI + Serilog + Versioning + Health + Middleware)

**What**: Configurar toda a injeção de dependência e middlewares no Program.cs
**Where**: `src/Api.Beneficiarios.WebAPI/Program.cs`
**Depends on**: T4, T5, T6
**Reuses**: Configuração existente de DbContext, Controllers, Swagger, CORS
**Requirement**: BEM-01, BEM-03, BEM-04, BEM-05, BEM-06

**O que configurar:**
1. Substituir `AddScoped<IBeneficiarioService, BeneficiarioService>()` por `AddMediatR()` + `AddAutoMapper()` + `AddValidatorsFromAssembly()`
2. Adicionar `AddSerilog()` com output em JSON no console
3. Adicionar `AddApiVersioning()` com prefixo `/api/v{version:apiVersion}`
4. Adicionar `AddHealthChecks().AddNpgSql(connectionString)`
5. Registrar `ValidationBehavior` como pipeline do MediatR
6. Registrar `GlobalExceptionMiddleware` ANTES dos outros middlewares
7. Manter DbContext, Controllers, Swagger, CORS existentes

**IMPORTANTE**: NÃO remover os Services antigos ainda — código novo e antigo coexistem até o fim da migração.

**Done when**:
- [ ] `builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(...))` para Application
- [ ] `builder.Services.AddAutoMapper(typeof(BeneficiarioProfile))`
- [ ] `builder.Services.AddValidatorsFromAssembly(...)` para Application
- [ ] `builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>))`
- [ ] `builder.Services.AddApiVersioning()` + rota `/api/v{version:apiVersion}`
- [ ] `builder.Services.AddHealthChecks().AddNpgSql(connectionString)`
- [ ] `app.UseSerilogRequestLogging()` no pipeline
- [ ] `app.UseMiddleware<GlobalExceptionMiddleware>()` ANTES de `UseCors`
- [ ] `app.MapHealthChecks("/health")` depois dos controllers
- [ ] `dotnet build` compila

**Tests**: none (configuração testada via T17)
**Gate**: `dotnet build`
**Commit**: `feat: configurar MediatR, AutoMapper, FluentValidation, Serilog, HealthChecks, Versioning no Program.cs`

---

## Phase 2: Beneficiarios CQRS (Sequencial)

### T8: Criar Commands + Validators de Beneficiário

**What**: Criar os 3 Commands (Criar, Atualizar, Excluir) como records + seus Validators
**Where**:
- `src/Api.Beneficiarios.Application/Commands/Beneficiario/CriarBeneficiarioCommand.cs`
- `src/Api.Beneficiarios.Application/Commands/Beneficiario/AtualizarBeneficiarioCommand.cs`
- `src/Api.Beneficiarios.Application/Commands/Beneficiario/ExcluirBeneficiarioCommand.cs`
- `src/Api.Beneficiarios.Application/Validators/Beneficiario/CriarBeneficiarioValidator.cs`
- `src/Api.Beneficiarios.Application/Validators/Beneficiario/AtualizarBeneficiarioValidator.cs`
**Depends on**: T7
**Reuses**: `CreateBeneficiarioDto`, `UpdateBeneficiarioDto`, `BeneficiarioResponseDto`
**Requirement**: BEM-01, BEM-03

**Records:**
```csharp
// Criar
public record CriarBeneficiarioCommand(
    string Nome, string CPF, DateTime DataNascimento, Guid PlanoId
) : IRequest<BeneficiarioResponseDto>;

// Atualizar
public record AtualizarBeneficiarioCommand(
    Guid Id, string? Nome, DateTime? DataNascimento,
    string? Status, Guid? PlanoId
) : IRequest<BeneficiarioResponseDto?>;

// Excluir
public record ExcluirBeneficiarioCommand(Guid Id) : IRequest<bool>;
```

**Validators (FluentValidation):**
- `CriarBeneficiarioValidator`: Nome 3-150 chars, CPF 11 dígitos, DataNascimento não futura, PlanoId não vazio
- `AtualizarBeneficiarioValidator`: Id não vazio, Nome se preenchido 3-150 chars, DataNascimento se preenchida não futura

**Done when**:
- [ ] 3 Commands como records criados
- [ ] 2 Validators criados com todas as regras de validação
- [ ] `dotnet build` compila

**Tests**: none (validators testados via T17)
**Gate**: `dotnet build`
**Commit**: `feat: adicionar Commands e Validators de Beneficiario`

---

### T9: Criar Handlers dos Commands de Beneficiário

**What**: Criar os 3 Handlers que processam os Commands de Beneficiário
**Where**:
- `src/Api.Beneficiarios.Application/Commands/Beneficiario/CriarBeneficiarioCommandHandler.cs`
- `src/Api.Beneficiarios.Application/Commands/Beneficiario/AtualizarBeneficiarioCommandHandler.cs`
- `src/Api.Beneficiarios.Application/Commands/Beneficiario/ExcluirBeneficiarioCommandHandler.cs`
**Depends on**: T8
**Reuses**: `IBeneficiarioRepository`, `IPlanoRepository`, `Beneficiario` entity, AutoMapper Profiles de T4
**Requirement**: BEM-01

**Lógica dos handlers (mesma dos services antigos, portada):**

`CriarBeneficiarioCommandHandler`:
1. Validar se CPF já existe → `BusinessRuleException` se duplicado
2. Validar se PlanoId existe → `NotFoundException` se não
3. Mapear Command → Entity (AutoMapper)
4. Chamar `_repository.AddAsync()`
5. Mapear Entity → ResponseDto (AutoMapper)
6. Retornar ResponseDto

`AtualizarBeneficiarioCommandHandler`:
1. Buscar entidade por Id → `NotFoundException` se não existir
2. Mapear Command → Entity (AutoMapper, só campos não-nulos)
3. Chamar `_repository.Update()`
4. Mapear → ResponseDto
5. Retornar ResponseDto

`ExcluirBeneficiarioCommandHandler`:
1. Buscar entidade por Id → `NotFoundException` se não existir
2. Chamar `entity.ExcluirSuavemente()`
3. Chamar `_repository.Update()`
4. Retornar `true`

**Done when**:
- [ ] 3 Handlers criados (1 classe cada)
- [ ] Injetam `IBeneficiarioRepository`, `IPlanoRepository`, `IMapper`
- [ ] Lançam exceções de domínio apropriadas
- [ ] Usam AutoMapper para mapeamento
- [ ] `dotnet build` compila

**Tests**: unit (co-localizado — mesmo task)
**Gate**: `dotnet test tests/Api.Beneficiarios.Tests.Unit/`
**Commit**: `feat: adicionar Command Handlers de Beneficiario`

---

### T10: Criar Queries + Handlers de Beneficiário (listagem + por Id)

**What**: Criar Queries (records) e seus Handlers para leitura de beneficiários
**Where**:
- `src/Api.Beneficiarios.Application/Queries/Beneficiario/ObterBeneficiariosQuery.cs` + Handler
- `src/Api.Beneficiarios.Application/Queries/Beneficiario/ObterBeneficiarioPorIdQuery.cs` + Handler
**Depends on**: T9
**Reuses**: `IBeneficiarioRepository`, DTOs, AutoMapper, `PaginatedResponse`
**Requirement**: BEM-01, BEM-06

**Records:**
```csharp
public record ObterBeneficiariosQuery(
    string? Status, Guid? PlanoId, int Page = 1, int PageSize = 10
) : IRequest<PaginatedResponse<BeneficiarioResponseDto>>;

public record ObterBeneficiarioPorIdQuery(Guid Id) : IRequest<BeneficiarioResponseDto?>;
```

**Handler de listagem:**
1. Aplicar filtros (Status, PlanoId) na query do repositório
2. Contar total de registros
3. Aplicar paginação (Skip/Take)
4. Mapear Entity → ResponseDto (AutoMapper)
5. Retornar `PaginatedResponse` com metadados

**Handler por Id:**
1. Buscar por Id via repositório
2. Se null → retornar null (controller retorna 404)
3. Mapear → ResponseDto
4. Retornar

**Done when**:
- [ ] 2 Query records criados
- [ ] 2 Handlers criados
- [ ] Paginação implementada com `PaginatedResponse`
- [ ] Filtros de Status e PlanoId funcionando
- [ ] `dotnet build` compila

**Tests**: unit (co-localizado — mesmo task)
**Gate**: `dotnet test tests/Api.Beneficiarios.Tests.Unit/`
**Commit**: `feat: adicionar Queries e Handlers de consulta de Beneficiarios`

---

### T11: Refatorar BeneficiarioController para usar MediatR

**What**: Substituir injeção de `IBeneficiarioService` por `IMediator` no controller
**Where**: `src/Api.Beneficiarios.WebAPI/Controllers/BeneficiarioController.cs`
**Depends on**: T9, T10
**Reuses**: Mesmo controller, mesmas rotas, mesmo contrato
**Requirement**: BEM-01

**Alterações:**
- Trocar `IBeneficiarioService _beneficiarioService` por `IMediator _mediator`
- Cada action chama `_mediator.Send(command/query)` no lugar de `_service.Metodo()`
- **Sem try-catch** — o `GlobalExceptionMiddleware` cuida disso
- Rota com versão: trocar `[Route("api/[controller]")]` para incluir versionamento
- Retornos mantidos: `CreatedAtAction`, `Ok`, `NoContent`

**Done when**:
- [ ] Controller não injeta mais `IBeneficiarioService`
- [ ] Controller injeta `IMediator` e chama `.Send()`
- [ ] Zero blocos try-catch no controller
- [ ] Rotas versionadas: `/api/v{version:apiVersion}/Beneficiario`
- [ ] `dotnet build` compila

**Tests**: unit (opcional neste momento, coberto via T17)
**Gate**: `dotnet build`
**Commit**: `feat: refatorar BeneficiarioController para MediatR CQRS`

---

## Phase 3: Planos CQRS (Sequencial)

### T12: Criar Commands + Validators de Plano

**What**: Criar os 3 Commands de Plano (Criar, Atualizar, Excluir) como records + Validators
**Where**:
- `src/Api.Beneficiarios.Application/Commands/Plano/CriarPlanoCommand.cs`
- `src/Api.Beneficiarios.Application/Commands/Plano/AtualizarPlanoCommand.cs`
- `src/Api.Beneficiarios.Application/Commands/Plano/ExcluirPlanoCommand.cs`
- `src/Api.Beneficiarios.Application/Validators/Plano/CriarPlanoValidator.cs`
- `src/Api.Beneficiarios.Application/Validators/Plano/AtualizarPlanoValidator.cs`
**Depends on**: T11
**Reuses**: `CreatePlanoDto`, `UpdatePlanoDto`, `PlanoResponseDto`
**Requirement**: BEM-02, BEM-03

**Records:**
```csharp
public record CriarPlanoCommand(string NomePlano, string CodRegistroAns) : IRequest<PlanoResponseDto>;
public record AtualizarPlanoCommand(Guid Id, string? NomePlano, string? CodRegistroAns, bool? StatusPlano) : IRequest<PlanoResponseDto?>;
public record ExcluirPlanoCommand(Guid Id) : IRequest<bool>;
```

**Validators:**
- `CriarPlanoValidator`: NomePlano 1-100 chars, CodRegistroAns 1-50 chars
- `AtualizarPlanoValidator`: Id não vazio, validações condicionais se campos preenchidos

**Done when**:
- [ ] 3 Commands + 2 Validators criados
- [ ] `dotnet build` compila

**Tests**: none (validators testados via T17)
**Gate**: `dotnet build`
**Commit**: `feat: adicionar Commands e Validators de Plano`

---

### T13: Criar Handlers dos Commands de Plano

**What**: Criar Handlers para Criar, Atualizar e Excluir Plano
**Where**:
- `src/Api.Beneficiarios.Application/Commands/Plano/CriarPlanoCommandHandler.cs`
- `src/Api.Beneficiarios.Application/Commands/Plano/AtualizarPlanoCommandHandler.cs`
- `src/Api.Beneficiarios.Application/Commands/Plano/ExcluirPlanoCommandHandler.cs`
**Depends on**: T12
**Reuses**: `IPlanoRepository`, `IBeneficiarioRepository`, `Plano` entity, AutoMapper
**Requirement**: BEM-02

**Lógica do ExcluirPlanoCommandHandler:**
- Verificar se há beneficiários vinculados antes de excluir
- Se houver → `BusinessRuleException` com mensagem "Plano possui beneficiários vinculados"

**Done when**:
- [ ] 3 Handlers criados
- [ ] Regra de negócio: não pode excluir plano com beneficiários
- [ ] Usam AutoMapper para mapeamento
- [ ] `dotnet build` compila

**Tests**: unit (co-localizado — mesmo task)
**Gate**: `dotnet test tests/Api.Beneficiarios.Tests.Unit/`
**Commit**: `feat: adicionar Command Handlers de Plano`

---

### T14: Criar Queries + Handlers de Plano (listagem + por Id)

**What**: Criar Queries e Handlers de leitura de planos
**Where**:
- `src/Api.Beneficiarios.Application/Queries/Plano/ObterPlanosQuery.cs` + Handler
- `src/Api.Beneficiarios.Application/Queries/Plano/ObterPlanoPorIdQuery.cs` + Handler
**Depends on**: T13
**Reuses**: `IPlanoRepository`, DTOs, AutoMapper
**Requirement**: BEM-02

**Records:**
```csharp
public record ObterPlanosQuery(int Page = 1, int PageSize = 10) : IRequest<PaginatedResponse<PlanoResponseDto>>;
public record ObterPlanoPorIdQuery(Guid Id) : IRequest<PlanoResponseDto?>;
```

**Done when**:
- [ ] 2 Query records + 2 Handlers criados
- [ ] Paginação na listagem
- [ ] `dotnet build` compila

**Tests**: unit (co-localizado — mesmo task)
**Gate**: `dotnet test tests/Api.Beneficiarios.Tests.Unit/`
**Commit**: `feat: adicionar Queries e Handlers de consulta de Planos`

---

### T15: Refatorar PlanoController para usar MediatR

**What**: Substituir `IPlanoService` por `IMediator` no PlanoController
**Where**: `src/Api.Beneficiarios.WebAPI/Controllers/PlanoController.cs`
**Depends on**: T13, T14
**Reuses**: Mesmo controller, mesmas rotas
**Requirement**: BEM-02

**Done when**:
- [ ] Controller usa `IMediator` no lugar de `IPlanoService`
- [ ] Zero try-catch
- [ ] Rotas versionadas
- [ ] `dotnet build` compila

**Tests**: none (testado via T17)
**Gate**: `dotnet build`
**Commit**: `feat: refatorar PlanoController para MediatR CQRS`

---

## Phase 4: Clean-up e Gate Final

### T16: Remover Services antigos

**What**: Remover Services, suas interfaces, e referências do DI
**Where**:
- Excluir: `src/Api.Beneficiarios.Application/Services/BeneficiarioService.cs`
- Excluir: `src/Api.Beneficiarios.Application/Services/PlanoService.cs`
- Excluir: `src/Api.Beneficiarios.Application/Services/Interfaces/IBeneficiarioService.cs`
- Excluir: `src/Api.Beneficiarios.Application/Services/Interfaces/IPlanoService.cs`
- Editar: `Program.cs` — remover `AddScoped` das interfaces de serviço (se ainda existirem)
**Depends on**: T15
**Reuses**: N/A
**Requirement**: BEM-01, BEM-02

**Done when**:
- [ ] 4 arquivos de serviço excluídos
- [ ] DI em Program.cs limpo (sem referências a Services)
- [ ] `dotnet build` compila sem erros
- [ ] Nenhum `using Api.Beneficiarios.Application.Services` em lugar nenhum

**Tests**: none
**Gate**: `dotnet build`
**Commit**: `refactor: remover Services antigos (substituídos por CQRS)`

---

### T17: Atualizar e rodar testes unitários + Gate Check Final

**What**: Rewrite dos testes unitários existentes para cobrir os novos Handlers, validar que tudo compila e testa
**Where**: `tests/Api.Beneficiarios.Tests.Unit/Application/`
**Depends on**: T16
**Reuses**: Estrutura de testes existente (xUnit + Moq + FluentAssertions)
**Requirement**: BEM-01 a BEM-06

**O que fazer:**
1. Remover testes de Services antigos (`BeneficiarioServiceTests.cs`, `PlanoServiceTests.cs`)
2. Criar testes para Command Handlers (mock dos repositórios, verificar chamadas e retornos)
3. Criar testes para Query Handlers (mock dos repositórios, verificar filtros e paginação)
4. Criar testes para Validators (FluentValidation testa isoladamente)
5. Rodar `dotnet build` e `dotnet test` — todos devem passar

**Estrutura de teste sugerida:**
```csharp
[Fact]
public async Task CriarBeneficiario_ComCPFDuplicado_DeveLancarBusinessRuleException()
{
    // Arrange
    _repoMock.Setup(r => r.ObterPorCPFAsync("12345678901")).ReturnsAsync(new Beneficiario());
    // Act
    var act = () => _handler.Handle(command, CancellationToken.None);
    // Assert
    await act.Should().ThrowAsync<BusinessRuleException>();
}
```

**Done when**:
- [ ] Serviços antigos e seus testes removidos
- [ ] Testes de handlers criados e passando
- [ ] Testes de validators criados e passando
- [ ] `dotnet build` passa sem warnings
- [ ] `dotnet test` passa com pelo menos a mesma cobertura de antes (~10+ testes)
- [ ] Swagger abre em dev mode com rotas `/api/v1/`

**Gate**: `dotnet build && dotnet test`
**Commit**: `test: atualizar testes unitários para CQRS (handlers + validators)`

---

## Parallel Execution Map

```
Phase 1 (Foundation):
  T1 ──→ T2 ──→ T3 ──→ T4 ──→ T5 ──→ T6 ──→ T7

Phase 2 (Beneficiarios):
  T7 ──→ T8 ──→ T9 ──→ T10 ──→ T11

Phase 3 (Planos):
  T11 ──→ T12 ──→ T13 ──→ T14 ──→ T15

Phase 4 (Clean-up):
  T15 ──→ T16 ──→ T17
```

Todas as fases são **sequenciais** porque cada task depende da anterior e as decisões de design são cumulativas.

---

## Diagram-Definition Cross-Check

| Task | Depends On (body) | Diagram Shows | Status |
|------|-------------------|---------------|--------|
| T1 | None | None (start) | ✅ Match |
| T2 | T1 | T1 → T2 | ✅ Match |
| T3 | T1 | T2 → T3 | ✅ Match |
| T4 | T2 | T3 → T4 | ✅ Match |
| T5 | T2 | T4 → T5 | ✅ Match |
| T6 | T2 | T5 → T6 | ✅ Match |
| T7 | T4, T5, T6 | T6 → T7 | ✅ Match |
| T8 | T7 | T7 → T8 | ✅ Match |
| T9 | T8 | T8 → T9 | ✅ Match |
| T10 | T9 | T9 → T10 | ✅ Match |
| T11 | T9, T10 | T10 → T11 | ✅ Match |
| T12 | T11 | T11 → T12 | ✅ Match |
| T13 | T12 | T12 → T13 | ✅ Match |
| T14 | T13 | T13 → T14 | ✅ Match |
| T15 | T13, T14 | T14 → T15 | ✅ Match |
| T16 | T15 | T15 → T16 | ✅ Match |
| T17 | T16 | T16 → T17 | ✅ Match |

---

## Test Co-location Validation

| Task | Code Layer Modified | Matrix Requires | Task Says | Status |
|------|-------------------|-----------------|-----------|--------|
| T1-T7 | Infra (csproj, Program.cs) | none/integration | none | ✅ OK (infra setup) |
| T8 | Application (Commands) | unit | none (tested via T17) | ⚠️ DEFER — validators testados em T17 |
| T9 | Application (Handlers) | unit | unit (co-located) | ✅ OK |
| T10 | Application (Queries + Handlers) | unit | unit (co-located) | ✅ OK |
| T11 | WebAPI (Controller) | unit | none (tested via T17) | ⚠️ DEFER — controller testado em T17 |
| T12 | Application (Commands) | unit | none (tested via T17) | ⚠️ DEFER — validators testados em T17 |
| T13 | Application (Handlers) | unit | unit (co-located) | ✅ OK |
| T14 | Application (Queries + Handlers) | unit | unit (co-located) | ✅ OK |
| T15 | WebAPI (Controller) | unit | none (tested via T17) | ⚠️ DEFER — controller testado em T17 |
| T16 | Application (removal) | N/A | none | ✅ OK (cleanup) |
| T17 | Application (tests) | unit | N/A (task é o próprio teste) | ✅ OK |

**Nota sobre os ⚠️ DEFER:** Validators e Controllers são simples o suficiente (Validators = regras declarativas, Controllers = 1 linha por action) que seus testes são mais eficazes agrupados em T17 do que espalhados em 5 tasks diferentes. Isso reduz duplicação de setup de teste.

---

## Resumo

| Fase | Tasks | O que entrega |
|------|-------|--------------|
| Phase 1 | T1-T7 (7 tasks) | Pacotes, exceções, paginação, AutoMapper, ValidationBehavior, Middleware, Program.cs |
| Phase 2 | T8-T11 (4 tasks) | Beneficiarios: Commands, Validators, Query Handlers, Command Handlers, Controller |
| Phase 3 | T12-T15 (4 tasks) | Planos: Commands, Validators, Query Handlers, Command Handlers, Controller |
| Phase 4 | T16-T17 (2 tasks) | Limpeza de Services antigos + Testes unitários + Gate check final |

**Total: 17 tasks**
