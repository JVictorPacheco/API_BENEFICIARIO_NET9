# API Beneficiário .NET 9

API REST desenvolvida em .NET 9 com PostgreSQL rodando em Docker.

## Requisitos

- Docker 20.10+
- Docker Compose 2.0+

## Estrutura do Projeto

```
.
├── src/
│   └── Api.Beneficiario/
│       ├── Controllers/          # Controllers da API
│       ├── Models/               # Modelos de dados
│       ├── Properties/           # Configurações do projeto
│       ├── Program.cs            # Ponto de entrada da aplicação
│       ├── appsettings.json      # Configurações da aplicação
│       └── Api.Beneficiario.csproj
├── docker-compose.yml            # Compose para desenvolvimento
├── docker-compose.prod.yml       # Compose para produção
├── Dockerfile                    # Dockerfile otimizado para produção
├── Dockerfile.dev                # Dockerfile com hot reload
└── README.md
```

## Como Executar

### Modo Desenvolvimento (com hot reload)

1. Clone o repositório:
```bash
git clone <seu-repositorio>
cd api_beneficiario_net
```

2. Inicie os containers:
```bash
docker-compose up -d
```

3. Verifique se os serviços estão rodando:
```bash
docker-compose ps
```

A API estará disponível em:
- **API**: http://localhost:5000
- **Swagger**: http://localhost:5000/swagger
- **Health Check**: http://localhost:5000/health
- **pgAdmin**: http://localhost:5050

### Credenciais de Acesso

**PostgreSQL:**
- Host: localhost
- Port: 5432
- Database: beneficiario_db
- Username: postgres
- Password: postgres

**pgAdmin:**
- URL: http://localhost:5050
- Email: admin@admin.com
- Password: admin

## Endpoints da API

### Beneficiários

- `GET /api/beneficiario` - Lista todos os beneficiários
- `GET /api/beneficiario/{id}` - Obtém um beneficiário por ID
- `POST /api/beneficiario` - Cria um novo beneficiário
- `PUT /api/beneficiario/{id}` - Atualiza um beneficiário
- `DELETE /api/beneficiario/{id}` - Remove um beneficiário

### Health Check

- `GET /health` - Verifica o status da API

## Comandos Úteis

### Visualizar logs
```bash
# Logs de todos os serviços
docker-compose logs -f

# Logs apenas da API
docker-compose logs -f api

# Logs apenas do PostgreSQL
docker-compose logs -f postgres
```

### Parar os serviços
```bash
docker-compose down
```

### Parar e remover volumes (limpar banco de dados)
```bash
docker-compose down -v
```

### Reconstruir os containers
```bash
docker-compose up -d --build
```

### Executar comandos dentro do container da API
```bash
# Acessar o bash do container
docker-compose exec api bash

# Executar migrations (quando configurado)
docker-compose exec api dotnet ef database update

# Restaurar pacotes
docker-compose exec api dotnet restore
```

### Acessar o banco de dados
```bash
docker-compose exec postgres psql -U postgres -d beneficiario_db
```

## Desenvolvimento

### Hot Reload

O projeto está configurado com hot reload. Qualquer alteração nos arquivos `.cs` será automaticamente detectada e a aplicação será recarregada.

### Adicionar um novo pacote NuGet

1. Entre no container:
```bash
docker-compose exec api bash
```

2. Adicione o pacote:
```bash
dotnet add package <nome-do-pacote>
```

3. Saia e reconstrua o container:
```bash
exit
docker-compose up -d --build
```

### Configurar Entity Framework Migrations

1. Instale o dotnet-ef no container (já está instalado no Dockerfile.dev):
```bash
docker-compose exec api dotnet ef migrations add InitialCreate
```

2. Aplique as migrations:
```bash
docker-compose exec api dotnet ef database update
```

## Modo Produção

Para executar em modo produção:

1. Crie um arquivo `.env` baseado no `.env.example`:
```bash
cp .env.example .env
```

2. Configure as variáveis de ambiente no arquivo `.env`

3. Execute com o compose de produção:
```bash
docker-compose -f docker-compose.prod.yml up -d
```

## Conectar pgAdmin ao PostgreSQL

1. Acesse http://localhost:5050
2. Faça login com as credenciais (admin@admin.com / admin)
3. Clique em "Add New Server"
4. Na aba "General":
   - Name: Beneficiario DB
5. Na aba "Connection":
   - Host: postgres
   - Port: 5432
   - Database: beneficiario_db
   - Username: postgres
   - Password: postgres
6. Clique em "Save"

## Troubleshooting

### Erro de conexão com o banco de dados
```bash
# Verifique se o PostgreSQL está rodando
docker-compose ps postgres

# Verifique os logs do PostgreSQL
docker-compose logs postgres

# Reinicie os containers
docker-compose restart
```

### Porta já em uso
Se a porta 5000, 5432 ou 5050 já estiver em uso, você pode alterá-la no `docker-compose.yml`:
```yaml
ports:
  - "NOVA_PORTA:5000"  # Para a API
```

### Limpar tudo e começar do zero
```bash
docker-compose down -v
docker-compose up -d --build
```

## Próximos Passos

- [ ] Implementar autenticação JWT
- [ ] Adicionar validações com FluentValidation
- [ ] Configurar migrations do Entity Framework
- [ ] Implementar testes unitários e de integração
- [ ] Adicionar logging estruturado com Serilog
- [ ] Configurar CI/CD
- [ ] Adicionar documentação Swagger mais detalhada

## Licença

MIT
# API_BENEFICIARIO_NET9
