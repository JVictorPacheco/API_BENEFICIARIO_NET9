# Backend CQRS + Modernização — Especificação

## Problem Statement

O backend atual usa o padrão Service (Services orquestrando repositórios), que funciona mas mistura leitura com escrita, acopla controllers a serviços e deixa cross-cutting concerns (validação, logging, tratamento de erro) espalhados pelo código. Para preparar o projeto para crescer (v2 com frontend React, autenticação, novas features), precisamos de uma arquitetura mais modular, testável e com responsabilidades bem definidas.

## Goals

- [x] Substituir Services por CQRS com MediatR (Commands/Queries/Handlers)
- [ ] Adicionar FluentValidation com pipeline behavior do MediatR
- [ ] Adicionar AutoMapper com Profiles
- [ ] Substituir try-catch nos controllers por Global Exception Handler
- [ ] Implementar Result Pattern com ProblemDetails (RFC 7807)
- [ ] Adicionar paginação nos endpoints GET
- [ ] Adicionar Serilog (structured logging)
- [ ] Adicionar Health Checks (banco de dados)
- [ ] Adicionar API Versioning (URL-based: /api/v1/...)

## Out of Scope

| Feature | Reason |
|---------|--------|
| Alterar schema do banco | Manter compatibilidade com dados existentes |
| Autenticação / autorização | Será feito na v2 |
| Frontend React | Feature separada, após backend |
| Trocar PostgreSQL por outro banco | PostgreSQL é o banco definido |

---

## User Stories

### P1: CQRS com MediatR — Beneficiários ⭐ MVP

**User Story**: Como desenvolvedor, quero que o CRUD de beneficiários use Commands e Queries separados via MediatR, substituindo o `BeneficiarioService`.

**Why P1**: É o core da migração. Define o padrão que será replicado para Planos. Sem isso, nada mais faz sentido.

**Acceptance Criteria**:

1. WHEN o controller recebe POST `/api/v1/Beneficiario` THEN o sistema SHALL enviar um `CriarBeneficiarioCommand` via MediatR e retornar o DTO de resposta
2. WHEN o controller recebe GET `/api/v1/Beneficiario` THEN o sistema SHALL enviar um `ObterBeneficiariosQuery` com filtros e retornar lista paginada
3. WHEN o controller recebe GET `/api/v1/Beneficiario/{id}` THEN o sistema SHALL enviar um `ObterBeneficiarioPorIdQuery` e retornar o DTO ou 404
4. WHEN o controller recebe PUT `/api/v1/Beneficiario/{id}` THEN o sistema SHALL enviar um `AtualizarBeneficiarioCommand` e retornar o DTO atualizado ou 404
5. WHEN o controller recebe DELETE `/api/v1/Beneficiario/{id}` THEN o sistema SHALL enviar um `ExcluirBeneficiarioCommand` e retornar 204 ou 404
6. WHEN um CPF duplicado é enviado THEN o handler SHALL lançar uma exceção de domínio mapeada para 409 Conflict

**Independent Test**: Subir a API, chamar cada endpoint com Swagger/curl e verificar que o comportamento é idêntico ao atual.

---

### P2: CQRS com MediatR — Planos

**User Story**: Como desenvolvedor, quero que o CRUD de planos siga o mesmo padrão CQRS dos beneficiários, substituindo o `PlanoService`.

**Why P2**: Segundo recurso da API. Menos complexo que beneficiários (sem filtros), serve para consolidar o padrão.

**Acceptance Criteria**:

1. WHEN o controller recebe POST `/api/v1/Plano` THEN o sistema SHALL enviar `CriarPlanoCommand` via MediatR
2. WHEN o controller recebe GET/PUT/DELETE `/api/v1/Plano/{id}` THEN o sistema SHALL usar os respectivos Commands/Queries
3. WHEN tenta excluir um plano com beneficiários vinculados THEN o sistema SHALL retornar 409 Conflict

**Independent Test**: Mesmos testes do P1, mas para endpoints de Plano.

---

### P3: FluentValidation + Pipeline Behavior

**User Story**: Como desenvolvedor, quero que a validação dos DTOs seja feita por FluentValidation, executada automaticamente via pipeline do MediatR, eliminando validação manual nos handlers.

**Why P3**: Separa validação da lógica de negócio. Reaproveitável, testável isoladamente, mensagens de erro padronizadas.

**Acceptance Criteria**:

1. WHEN um Command/Query é enviado THEN o ValidationBehavior SHALL executar todos os validators registrados automaticamente
2. WHEN a validação falha THEN o sistema SHALL retornar 400 Bad Request com lista de erros no formato ProblemDetails
3. WHEN a validação passa THEN o handler SHALL executar normalmente
4. Validators existentes: CPF (formato 11 dígitos, único), Nome (3-150 chars), DataNascimento (não futura), PlanoId (obrigatório, existente)

**Independent Test**: Enviar POST com CPF inválido → 400 com erros. Enviar POST com dados válidos → 201.

---

### P4: AutoMapper + Profiles

**User Story**: Como desenvolvedor, quero que o mapeamento Entity ↔ DTO seja feito por AutoMapper, eliminando mapeamento manual nos handlers.

**Why P4**: Reduz boilerplate, previne erros de mapeamento quando novas propriedades são adicionadas.

**Acceptance Criteria**:

1. WHEN um handler precisa mapear Entity → ResponseDto THEN o AutoMapper SHALL fazer o mapeamento via Profile configurado
2. WHEN um handler precisa mapear CreateDto/UpdateDto → Entity THEN o AutoMapper SHALL fazer o mapeamento
3. Todas as propriedades mapeadas anteriormente de forma manual SHALL ser cobertas pelos Profiles

**Independent Test**: Rodar testes unitários existentes com os mapeamentos do AutoMapper mockados.

---

### P5: Global Exception Handler + ProblemDetails

**User Story**: Como desenvolvedor, quero que exceções não tratadas e erros de domínio sejam capturados por um middleware global que retorne ProblemDetails (RFC 7807), eliminando try-catch dos controllers.

**Why P5**: Respostas de erro padronizadas, sem lógica de erro espalhada nos controllers.

**Acceptance Criteria**:

1. WHEN uma exceção de validação ocorre THEN o middleware SHALL retornar 400 com ProblemDetails
2. WHEN um recurso não é encontrado THEN o middleware SHALL retornar 404 com ProblemDetails
3. WHEN há conflito (CPF duplicado, exclusão de plano com dependentes) THEN o middleware SHALL retornar 409 com ProblemDetails
4. WHEN uma exceção não tratada ocorre THEN o middleware SHALL retornar 500 com ProblemDetails (sem stack trace em produção)
5. Controllers não devem conter blocos try-catch

**Independent Test**: Forçar cada tipo de erro e verificar o formato JSON da resposta.

---

### P6: Paginação, Serilog, Health Checks, API Versioning

**User Story**: Como desenvolvedor, quero funcionalidades de infraestrutura modernas (paginação, logs estruturados, health checks, versionamento de API).

**Why P6**: Prepara o projeto para produção. Menos prioritário que os anteriores porque não muda a arquitetura core.

**Acceptance Criteria**:

1. WHEN GET `/api/v1/Beneficiario` é chamado com `?page=1&pageSize=10` THEN o sistema SHALL retornar resultados paginados com metadados (total, página atual, total de páginas)
2. WHEN a API inicia THEN logs estruturados (JSON) com Serilog SHALL ser enviados para console
3. WHEN GET `/health` é chamado THEN o sistema SHALL retornar status do banco de dados
4. Todas as rotas devem usar prefixo `/api/v1/`

**Independent Test**: Chamar endpoint paginado, verificar logs no console, bater no /health, verificar rotas.

---

## Edge Cases

- WHEN o banco de dados está indisponível THEN o Health Check SHALL retornar `Unhealthy`
- WHEN pageSize > 100 THEN o sistema SHALL limitar a 100
- WHEN page < 1 THEN o sistema SHALL assumir page=1
- WHEN a connection string não está configurada THEN a API SHALL falhar no startup com mensagem clara
- WHEN data de nascimento é futura THEN a validação SHALL rejeitar
- WHEN CPF tem caracteres não numéricos THEN a validação SHALL rejeitar

---

## Requirement Traceability

| ID | Story | Status |
|----|-------|--------|
| BEM-01 | P1: CQRS Beneficiários | Pending |
| BEM-02 | P2: CQRS Planos | Pending |
| BEM-03 | P3: FluentValidation | Pending |
| BEM-04 | P4: AutoMapper | Pending |
| BEM-05 | P5: Global Exception Handler | Pending |
| BEM-06 | P6: Paginação, Serilog, Health, Versioning | Pending |

## Success Criteria

- [ ] Todos os endpoints existentes funcionam exatamente como antes (mesmo contrato)
- [ ] `dotnet build` passa sem warnings
- [ ] `dotnet test` passa com cobertura >= atual
- [ ] Swagger reflete as novas rotas versionadas
- [ ] Nenhum try-catch nos controllers
- [ ] Nenhuma referência aos Services antigos (BeneficiarioService, PlanoService)
