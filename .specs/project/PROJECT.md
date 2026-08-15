# API Beneficiários - Fullstack

**Vision:** Sistema fullstack para gerenciamento de beneficiários e planos de saúde, usando .NET 9 com Clean Architecture, CQRS e React no frontend.
**For:** Operadores de planos de saúde que precisam cadastrar, consultar e gerenciar beneficiários.
**Solves:** CRUD de beneficiários vinculados a planos de saúde, com validações, filtros e interface moderna.

## Goals

- Migrar backend para CQRS + MediatR + FluentValidation (qualidade de código)
- Garantir cobertura de testes (unitários + integração)
- Criar frontend React com TypeScript consumindo a API
- Manter compatibilidade com a base de dados PostgreSQL existente

## Tech Stack

**Core:**
- Framework: .NET 9.0 (backend) + React 19 (frontend)
- Language: C# 13 + TypeScript
- Database: PostgreSQL via EF Core 9

**Key dependencies (backend):** MediatR, FluentValidation, AutoMapper, Serilog, Asp.Versioning
**Key dependencies (frontend):** Vite, React Router, TanStack Query, Axios, Tailwind CSS, shadcn/ui

## Scope

**v1 includes:**
- Migrar backend para CQRS com MediatR (Commands/Queries/Handlers)
- Adicionar FluentValidation nos Commands/Queries
- Adicionar AutoMapper para Entity ↔ DTO
- Global Exception Handler com ProblemDetails (RFC 7807)
- Paginação nos endpoints de listagem
- Health checks + Serilog
- API versioning (URL-based)

**v2 includes (futuro):**
- Frontend React com todas as telas (CRUD Beneficiários, CRUD Planos)
- Autenticação e autorização

**Explicitly out of scope:**
- Autenticação/autorização (v2)
- Deploy em produção (v2)
- CI/CD pipeline (v2)
- Migração de banco de dados (manter o existente)

## Constraints

- Timeline: Sem pressa — foco em aprendizado
- Technical: Manter PostgreSQL existente, sem alterar schema
- Resources: 1 desenvolvedor (você), eu orientando
