# STATE.md — API Beneficiários

## Decisions

| Data | Decisão | Motivo |
|------|---------|--------|
| 2026-07-28 | Evoluir projeto existente (não criar do zero) | Preservar histórico, estrutura já boa |
| 2026-07-28 | Backend primeiro, frontend depois | Foco em um de cada vez |
| 2026-07-28 | MCPs configurados: context7, microsoft-learn, dotnet-context | Documentação e análise de código |

## Blockers

Nenhum no momento.

## Preferences

- Nomes em português (PT-BR) para métodos, variáveis, classes de domínio
- Commits atômicos por tarefa
- Verificar com `dotnet build && dotnet test` a cada etapa

## Todo

- [ ] Finalizar spec da feature de migração do backend

## Deferred Ideas

- Autenticação JWT (v2)
- Deploy em Kubernetes (v3)
- Mensageria com RabbitMQ/Kafka (v3)
