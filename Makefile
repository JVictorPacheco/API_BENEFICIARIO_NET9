.PHONY: help up down build logs restart clean ps exec-api exec-db migrate

help: ## Mostra esta mensagem de ajuda
	@echo "Comandos disponíveis:"
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | awk 'BEGIN {FS = ":.*?## "}; {printf "  \033[36m%-15s\033[0m %s\n", $$1, $$2}'

up: ## Inicia os containers em modo desenvolvimento
	docker-compose up -d

down: ## Para os containers
	docker-compose down

build: ## Reconstrói os containers
	docker-compose up -d --build

logs: ## Exibe os logs de todos os serviços
	docker-compose logs -f

logs-api: ## Exibe os logs apenas da API
	docker-compose logs -f api

logs-db: ## Exibe os logs apenas do PostgreSQL
	docker-compose logs -f postgres

restart: ## Reinicia os containers
	docker-compose restart

clean: ## Para os containers e remove volumes
	docker-compose down -v

ps: ## Lista os containers em execução
	docker-compose ps

exec-api: ## Acessa o bash do container da API
	docker-compose exec api bash

exec-db: ## Acessa o PostgreSQL
	docker-compose exec postgres psql -U postgres -d beneficiario_db

migrate-add: ## Cria uma nova migration (use NAME=NomeDaMigration)
	docker-compose exec api dotnet ef migrations add $(NAME)

migrate-update: ## Aplica as migrations no banco de dados
	docker-compose exec api dotnet ef database update

migrate-remove: ## Remove a última migration
	docker-compose exec api dotnet ef migrations remove

prod-up: ## Inicia em modo produção
	docker-compose -f docker-compose.prod.yml up -d

prod-down: ## Para o modo produção
	docker-compose -f docker-compose.prod.yml down

prod-logs: ## Exibe logs do modo produção
	docker-compose -f docker-compose.prod.yml logs -f
