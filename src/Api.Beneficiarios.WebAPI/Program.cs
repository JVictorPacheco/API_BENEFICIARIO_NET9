using Microsoft.EntityFrameworkCore;
using Api.Beneficiarios.Infraestructure.Data;
using Api.Beneficiarios.Infraestructure.Repositories;
using Api.Beneficiarios.Domain.Interfaces;

// Program.cs serve detalhadamente para configurar e iniciar a aplicação web ASP.NET Core, incluindo serviços, middlewares, e dependências necessárias para o funcionamento da API de beneficiários.


// var builder = WebApplication.CreateBuilder(args) é utilizado para iniciar a configuração da aplicação web.
var builder = WebApplication.CreateBuilder(args); // Serve para criar o objeto builder que configura a aplicação

// ========== CONFIGURAÇÃO DO BANCO DE DADOS ==========

// Pega connection string: prioriza variável de ambiente (.env), senão usa appsettings
var connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING")  // Serve para obter a string de conexão do banco de dados a partir de uma variável de ambiente
                       ?? builder.Configuration.GetConnectionString("DefaultConnection"); // Serve para obter a string de conexão com o banco de dados


if (string.IsNullOrEmpty(connectionString)) // Verifica se a string de conexão está vazia ou nula
{
    throw new InvalidOperationException("Connection string não configurada! Verifique appsettings.json ou variável de ambiente DATABASE_CONNECTION_STRING"); // Lança uma exceção se a string de conexão não estiver configurada corretamente
}


// Registra o DbContext com PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(connectionString);

    // Apenas em desenvolvimento: mostra SQL no console e detalhes de erros
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging(); // Mostra dados sensíveis no log (útil para debug)
        options.EnableDetailedErrors(); // Mostra detalhes dos erros do EF Core
    }
});

// ========== DEPENDENCY INJECTION - REPOSITORIES ==========


// Registra repositories com lifetime Scoped (uma instância por request HTTP)
builder.Services.AddScoped<IBeneficiarioRepository, BeneficiarioRepository>(); // Nessa linha de código, o repositório IBeneficiarioRepository está sendo registrado com uma implementação concreta BeneficiarioRepository no contêiner de injeção de dependência. O tempo de vida do repositório é definido como "Scoped", o que significa que uma nova instância do repositório será criada para cada solicitação HTTP. Isso é útil para garantir que os dados específicos da solicitação sejam mantidos isolados entre diferentes solicitações.

// Registra services com lifetime Scoped (uma instância por request HTTP)
// builder.Services.AddScoped<IBeneficiarioService, BeneficiarioService>(); // Nessa linha de codigo, o serviço IBeneficiarioService está sendo registrado com uma implementação concreta BeneficiarioService no contêiner de injeção de dependência. O tempo de vida do serviço é definido como "Scoped", o que significa que uma nova instância do serviço será criada para cada solicitação HTTP. Isso é útil para garantir que os dados específicos da solicitação sejam mantidos isolados entre diferentes solicitações.


// ========== CONFIGURAÇÃO DE CONTROLLERS ==========


// Toda essa seção debaixo configura os controllers da aplicação, incluindo opções de serialização JSON.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Retorna enums como string ("Ativo") ao invés de número (1)
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter()
        );

        // Ignora propriedades null no JSON de resposta (deixa JSON mais limpo)
        options.JsonSerializerOptions.DefaultIgnoredCondition =
            System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });


// ========== SWAGGER / OPENAPI ==========


// Adiciona serviços para geração de documentação OpenAPI/Swagger
// Esta seção configura o Swagger para a API, incluindo detalhes como título, versão, descrição e informações de contato.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "API Beneficiários - Plano de Saúde",
        Version = "v1",
        Description = "API REST para gerenciar beneficiários e planos de saúde.\n\n" +
                      "**Funcionalidades:**\n" +
                      "- CRUD completo de Beneficiários\n" +
                      "- CRUD completo de Planos\n" +
                      "- Validação de CPF único\n" +
                      "- Soft delete (exclusão lógica)\n" +
                      "- Filtros por status e plano",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "João Victor Maciel Pacheco",
            Email = "joaovictormacielpacheco@gmail.com",
            Url = new Uri("https://github.com/JVictorPacheco/API_BENEFICIARIO_NET9")
        }
    });

        // Adiciona comentários XML ao Swagger (opcional - bônus!)
        // Você precisará habilitar geração de XML no .csproj
        // var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
        // var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        // if (File.Exists(xmlPath))
        // {
        //     c.IncludeXmlComments(xmlPath);
        // }
});


    // ========== CORS (Cross-Origin - para frontend) ==========

// Na seguinte seção, são configuradas as políticas de CORS (Cross-Origin Resource Sharing) para permitir que o frontend acesse a API a partir de diferentes origens.
builder.Services.AddCors(options => // Adiciona serviços de CORS para permitir requisições de diferentes origens (útil para frontends)
{
    options.AddPolicy("AllowAll", policy => // Define uma política chamada "AllowAll" que é bastante permissiva para CORS
    {
        policy.AllowAnyOrigin() // Permite requisições de qualquer origem
              .AllowAnyMethod() // Permite qualquer método HTTP (GET, POST, etc.)
              .AllowAnyHeader(); // Permite qualquer cabeçalho
    });
}); 


   // ========== BUILD DA APLICAÇÃO ==========
var app = builder.Build(); // Constrói a aplicação com as configurações definidas no builder

   // ========== MIDDLEWARES ==========

// Configura middlewares da aplicação (pipeline de requisições)
// Swagger: APENAS em desenvolvimento (segurança)
if (app.Environment.IsDevelopment()) // Verifica se o ambiente é de desenvolvimento
{
    app.UseSwagger(); // Habilita o middleware do Swagger para gerar a documentação da API
    app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Beneficiários v1"); // Define o endpoint do Swagger UI
            c.RoutePrefix = string.Empty; // Define o Swagger UI na raiz da aplicação
            c.DocumentTitle = " Api Beneficiarios - Documentação "; // Define o título do documento Swagger UI
        });
}


app.UseCors("AllowAll"); // Aplica a política de CORS "AllowAll" definida anteriormente
app.UseHttpsRedirection(); // Redireciona requisições HTTP para HTTPS
app.UseAuthorization(); // Habilita o middleware de autorização (autenticação não implementada aqui)
app.MapControllers(); // Mapeia os controllers para endpoints da API


// ============   LOGGING DE INICIALIZAÇÃO  ===========

app.Logger.LogInformation("🚀 Api beneficiarios iniciada!"); // Registra uma mensagem de log informando que a aplicação foi iniciada com sucesso
app.Logger.LogInformation($"📊 Ambiente: {app.Environment.EnvironmentName}"); // Registra o ambiente atual (Desenvolvimento, Produção, etc.)
app.Logger.LogInformation($"🗄️ Banco de dados{(connectionString.Contains("localhost") ? "local (Dokcer)" : "(Produção)")}");


if (app.Environment.IsDevelopment())
{
    app.Logger.LogInformation("📖 Swagger disponível em: http://localhost:5000/");
}

// ============ EXECUÇÃO DA APLICAÇÃO ==========

app.Run(); // Inicia a aplicação e começa a escutar requisições HTTP

