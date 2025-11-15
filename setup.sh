#!/bin/bash

echo "=========================================="
echo "  API Beneficiário - Setup Inicial"
echo "=========================================="
echo ""

# Verifica se o Docker está instalado
if ! command -v docker &> /dev/null; then
    echo "❌ Docker não está instalado. Por favor, instale o Docker primeiro."
    exit 1
fi

# Verifica se o Docker Compose está instalado
if ! command -v docker-compose &> /dev/null; then
    echo "❌ Docker Compose não está instalado. Por favor, instale o Docker Compose primeiro."
    exit 1
fi

echo "✅ Docker e Docker Compose detectados"
echo ""

# Cria arquivo .env se não existir
if [ ! -f .env ]; then
    echo "📝 Criando arquivo .env..."
    cp .env.example .env
    echo "✅ Arquivo .env criado"
else
    echo "ℹ️  Arquivo .env já existe"
fi
echo ""

# Inicia os containers
echo "🚀 Iniciando os containers..."
docker-compose up -d --build

echo ""
echo "⏳ Aguardando os serviços ficarem prontos..."
sleep 10

# Verifica se os containers estão rodando
if docker-compose ps | grep -q "Up"; then
    echo ""
    echo "=========================================="
    echo "  ✅ Setup concluído com sucesso!"
    echo "=========================================="
    echo ""
    echo "Serviços disponíveis:"
    echo "  📡 API: http://localhost:5000"
    echo "  📚 Swagger: http://localhost:5000/swagger"
    echo "  ❤️  Health Check: http://localhost:5000/health"
    echo "  🗄️  pgAdmin: http://localhost:5050"
    echo ""
    echo "Credenciais do pgAdmin:"
    echo "  Email: admin@admin.com"
    echo "  Senha: admin"
    echo ""
    echo "Credenciais do PostgreSQL:"
    echo "  Host: localhost (ou 'postgres' dentro do Docker)"
    echo "  Port: 5432"
    echo "  Database: beneficiario_db"
    echo "  Username: postgres"
    echo "  Password: postgres"
    echo ""
    echo "Comandos úteis:"
    echo "  make help      - Ver todos os comandos disponíveis"
    echo "  make logs      - Ver logs de todos os serviços"
    echo "  make logs-api  - Ver logs apenas da API"
    echo "  make down      - Parar os serviços"
    echo ""
else
    echo ""
    echo "❌ Houve um problema ao iniciar os containers"
    echo "Execute 'docker-compose logs' para ver os detalhes"
    exit 1
fi
