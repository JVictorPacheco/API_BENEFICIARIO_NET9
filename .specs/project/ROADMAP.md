# Roadmap

## v1 — Backend Modernization (em andamento)

### Feature: Migração para CQRS + MediatR + Boas Práticas
**Status:** Em especificação

- Substituir Services por Commands/Queries com MediatR
- FluentValidation, AutoMapper, Global Exception Handler
- Paginação, Serilog, Health Checks, API Versioning

---

## v2 — Frontend React

### Feature: Telas CRUD Beneficiários e Planos
**Status:** Planejado

- Vite + React 19 + TypeScript + Tailwind CSS + shadcn/ui
- React Router v7, TanStack Query v5, Axios, React Hook Form + Zod
- Lista paginada com filtros, formulários de criação/edição, exclusão com confirmação

---

## v3 — Autenticação e Auditoria

### Feature: Login, JWT e Trilha de Auditoria
**Status:** Ideia

- ASP.NET Core Identity + JWT Bearer (roles: admin, operador)
- Login no frontend com protected routes, refresh token
- EF Core interceptors para `AuditLog` (quem alterou o quê e quando)
- Tabela de histórico na UI

---

## v4 — Funcionalidades de Negócio Avançadas

### Feature: Documentos, Importação e Dashboard
**Status:** Ideia

- Upload de documentos (RG, comprovante de residência) com validação
- Importação CSV/Excel de beneficiários em lote com relatório de erros
- Exportação de relatórios em PDF/Excel
- Dashboard com gráficos (beneficiários por plano, ativos vs inativos)

---

## v5 — Infra e DevOps

### Feature: CI/CD, Observabilidade e Background Jobs
**Status:** Ideia

- CI/CD com GitHub Actions (build, test, deploy)
- OpenTelemetry + Grafana + Prometheus para observabilidade
- Background Jobs com Hangfire (envio de email, limpeza de dados expirados)
- Testes de integração com Testcontainers (PostgreSQL real)

---

## v6 — Arquitetura Distribuída

### Feature: Event-Driven, Cache e Multi-Tenancy
**Status:** Ideia

- Domain Events com RabbitMQ ou Azure Service Bus
- Cache distribuído com Redis
- API Gateway / BFF com YARP
- Multi-tenancy (múltiplas operadoras no mesmo sistema)
- App mobile com React Native (opcional)
