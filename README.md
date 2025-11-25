# API Beneficiários - Desafio Técnico

API REST para gerenciar beneficiários de plano de saúde, desenvolvida em .NET 9 com Clean Architecture.

## Visão Geral

Sistema completo de CRUD para gerenciar **Planos** e **Beneficiários** de plano de saúde, com:

- Validação de CPF único
- Soft delete (exclusão lógica)
- Filtros por status e plano
- Documentação via Swagger

## Stack Utilizada

| Tecnologia | Versão | Uso |
|------------|--------|-----|
| .NET | 9.0 | Framework principal |
| PostgreSQL | 16 | Banco de dados |
| Entity Framework Core | 9.0 | ORM |
| xUnit | 2.9 | Testes unitários |
| Moq | 4.20 | Mock para testes |
| FluentAssertions | 6.12 | Assertions nos testes |
| Docker | - | Containerização |
| Swagger | - | Documentação da API |

## Estrutura do Projeto (Clean Architecture)

```
src/
├── Api.Beneficiarios.Domain/           # Entidades e interfaces
│   ├── Entities/
│   ├── Enums/
│   └── Interfaces/
├── Api.Beneficiarios.Application/      # DTOs e Services
│   ├── DTOs/
│   └── Services/
├── Api.Beneficiarios.Infrastructure/   # Repositórios e DbContext
│   ├── Data/
│   ├── Configurations/
│   ├── Migrations/
│   └── Repositories/
└── Api.Beneficiarios.WebAPI/           # Controllers e Program.cs
    └── Controllers/

tests/
└── Api.Beneficiarios.Tests.Unit/       # Testes unitários
    └── Application/Services/
```

## Como Rodar

### Pré-requisitos

- .NET 9 SDK
- PostgreSQL 16 (ou Docker)
- Docker e Docker Compose (opcional)

### Opção 1: Rodar Local (sem Docker)

1. Clone o repositório:
```bash
git clone https://github.com/seu-usuario/api-beneficiarios.git
cd api-beneficiarios
```

2. Configure a connection string no `appsettings.json`:
```json
{
  "DatabaseConnection": {
    "ConnectionString": "Host=localhost;Port=5432;Database=beneficiario_db;Username=postgres;Password=sua_senha"
  }
}
```

3. Aplique as migrations:
```bash
dotnet ef database update --project src/Api.Beneficiarios.Infrastructure --startup-project src/Api.Beneficiarios.WebAPI
```

4. Execute a aplicação:
```bash
dotnet run --project src/Api.Beneficiarios.WebAPI
```

5. Acesse: http://localhost:5000/swagger

### Opção 2: Rodar com Docker

1. Suba os containers:
```bash
docker-compose up --build
```

2. Em outro terminal, aplique as migrations:
```bash
dotnet ef database update --project src/Api.Beneficiarios.Infrastructure --startup-project src/Api.Beneficiarios.WebAPI --connection "Host=localhost;Port=5432;Database=beneficiario_db;Username=postgres;Password=postgresql"
```

3. Acesse:
   - **API/Swagger**: http://localhost:5000/swagger
   - **pgAdmin**: http://localhost:5050 (admin@admin.com / admin)

### Parar os containers
```bash
docker-compose down
```

## Como Rodar os Testes

```bash
dotnet test
```

Resultado esperado: **20 testes passando** (10 de BeneficiarioService + 10 de PlanoService)

## Endpoints da API

### Planos

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| POST | `/api/plano` | Criar plano |
| GET | `/api/plano` | Listar todos |
| GET | `/api/plano/{id}` | Buscar por ID |
| PUT | `/api/plano/{id}` | Atualizar |
| DELETE | `/api/plano/{id}` | Excluir (soft delete) |

### Beneficiários

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| POST | `/api/beneficiario` | Criar beneficiário |
| GET | `/api/beneficiario` | Listar todos (com filtros) |
| GET | `/api/beneficiario/{id}` | Buscar por ID |
| PUT | `/api/beneficiario/{id}` | Atualizar |
| DELETE | `/api/beneficiario/{id}` | Excluir (soft delete) |

**Filtros disponíveis** em `GET /api/beneficiario`:
- `?status=ATIVO` ou `?status=INATIVO`
- `?planoId={guid}`

## Exemplos de Requisições

### Criar Plano
```bash
curl -X POST http://localhost:5000/api/plano \
  -H "Content-Type: application/json" \
  -d '{
    "nomePlano": "Plano Ouro",
    "codRegistroAns": "ANS-123456"
  }'
```

### Criar Beneficiário
```bash
curl -X POST http://localhost:5000/api/beneficiario \
  -H "Content-Type: application/json" \
  -d '{
    "nome": "João Silva",
    "cpf": "12345678909",
    "dataNascimento": "1990-05-20",
    "planoId": "uuid-do-plano-aqui"
  }'
```

### Listar Beneficiários Ativos
```bash
curl http://localhost:5000/api/beneficiario?status=ATIVO
```

### Atualizar Status para Inativo
```bash
curl -X PUT http://localhost:5000/api/beneficiario/{id} \
  -H "Content-Type: application/json" \
  -d '{
    "status": "INATIVO"
  }'
```

### Excluir Beneficiário
```bash
curl -X DELETE http://localhost:5000/api/beneficiario/{id}
```

## Decisões de Projeto

### Por que Clean Architecture?
Escolhi Clean Architecture para manter o código organizado e separar responsabilidades. Facilita testes e manutenção.

### Soft Delete vs Hard Delete
Optei por soft delete (campo `Excluido`) para manter histórico dos dados. Registros excluídos não aparecem nas listagens mas continuam no banco.

### Validação de CPF
Implementei validação básica (11 dígitos numéricos e unicidade). A validação de dígitos verificadores pode ser adicionada futuramente.

### Nullable DTOs no Update
Os DTOs de atualização têm campos nullable para permitir atualização parcial (enviar só os campos que deseja alterar).

### StatusPlano vs Excluido
São conceitos diferentes:
- `StatusPlano`: controle comercial (plano ativo/inativo para novos cadastros)
- `Excluido`: soft delete (registro foi removido do sistema)

## Credenciais

**PostgreSQL (Docker)**
- Host: localhost
- Porta: 5432
- Database: beneficiario_db
- Usuário: postgres
- Senha: postgresql

**pgAdmin**
- URL: http://localhost:5050
- Email: admin@admin.com
- Senha: admin

## Autor

João Pacheco
