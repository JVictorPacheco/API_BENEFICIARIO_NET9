// Resumidamento o arquivo program.cs é o ponto de entrada da aplicação, onde configuramos os serviços, middlewares e a 
// pipeline de requisições. Ele utiliza o WebApplicationBuilder para configurar a aplicação, adicionando serviços como Entity 
// Framework Core, Serilog, MediatR, AutoMapper, FluentValidation, repositórios e serviços de domínio. Além disso, ele 
// configura o versionamento da API, health checks, CORS e Swagger para documentação da API. Por fim, ele constrói e executa 
// a aplicação.


// Builder. é a instância do WebApplicationBuilder que é usada para configurar a aplicação, adicionando serviços, middlewares e
// outras configurações necessárias antes de construir a aplicação final.

// App. é a instância do WebApplication que representa a aplicação web em execução. Ele é criado a partir do builder e é usado
// para configurar a pipeline de requisições, middlewares e rotas da aplicação antes de iniciar a execução do servidor web.

// Uma das principais diferenças entre o builder e o app é que o builder é usado para configurar a aplicação antes de ser
// construída, enquanto o app é usado para configurar a aplicação depois de construída e antes de ser executada. O builder é 
// usado para adicionar serviços, middlewares e outras configurações, enquanto o app é usado para configurar a pipeline de 
// requisições, middlewares e rotas da aplicação. 


using Microsoft.EntityFrameworkCore;
using Api.Beneficiarios.Infrastructure.Data;
using Api.Beneficiarios.Infrastructure.Repositories;
using Api.Beneficiarios.Domain.Interfaces;
using Api.Beneficiarios.Application.Services;
using Api.Beneficiarios.Application.Services.Interfaces;
using FluentValidation;
using MediatR;
using Serilog;
using Api.Beneficiarios.Application.Behaviors;
using Api.Beneficiarios.Application.Mappings;
using Api.Beneficiarios.WebAPI.Middleware;


var builder = WebApplication.CreateBuilder(args); 


var connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING")  
                       ?? builder.Configuration.GetConnectionString("DefaultConnection"); 


if (string.IsNullOrEmpty(connectionString)) 
{
    throw new InvalidOperationException("Connection string não configurada! Verifique appsettings.json ou variável de ambiente DATABASE_CONNECTION_STRING"); 
}



builder.Services.AddDbContext<AppDbContext>(options => // Adiciona o contexto do banco de dados usando a connection string configurada
{
    options.UseNpgsql(connectionString); // Configura o Entity Framework Core para usar o PostgreSQL com a connection string fornecida

    
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging(); 
        options.EnableDetailedErrors(); 
    }
});

// Configuração do Serilog
builder.Host.UseSerilog((context, configuration) => // Configura o Serilog para registrar logs
{
    configuration.ReadFrom.Configuration(context.Configuration) // Lê a configuração do Serilog do appsettings.json
                 .Enrich.FromLogContext() // Adiciona informações de contexto aos logs
                 .WriteTo.Console(); // Escreve os logs no console
});


// Aqui seria os builders do MediatR, AutoMapper, FluentValidation, Repositories e Services
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(BeneficiarioProfile).Assembly)); // Adiciona o MediatR e registra os handlers do assembly atual
builder.Services.AddAutoMapper(typeof(BeneficiarioProfile).Assembly); // Adiciona o AutoMapper e registra os profiles do assembly atual
builder.Services.AddValidatorsFromAssembly(typeof(BeneficiarioProfile).Assembly); // Adiciona os validadores do FluentValidation do assembly atual
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>)); // Adiciona o pipeline de validação do MediatR
builder.Services.AddScoped<IBeneficiarioRepository, BeneficiarioRepository>(); 
builder.Services.AddScoped<IPlanoRepository, PlanoRepository>();

builder.Services.AddScoped<IBeneficiarioService, BeneficiarioService>();
builder.Services.AddScoped<IPlanoService, PlanoService>();


// Api Versioning
builder.Services.AddApiVersioning(options => // Adiciona o versionamento da API
{
    options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0); // Define a versão padrão da API como 1.0
    options.AssumeDefaultVersionWhenUnspecified = true; // Assume a versão padrão quando não especificada
    options.ReportApiVersions = true; // Adiciona cabeçalhos de versão da API na resposta
    options.ApiVersionReader = new Asp.Versioning.UrlSegmentApiVersionReader(); // Lê a versão da URL, ex: /api/v1/beneficiarios
});


builder.Services.AddHealthChecks().AddNpgSql(connectionString); // Adiciona o health check para o banco de dados PostgreSQL usando a connection string configurada


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter()
        );

        options.JsonSerializerOptions.DefaultIgnoreCondition =
            System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });



builder.Services.AddEndpointsApiExplorer();   // Adiciona o suporte para explorar os endpoints da API, necessário para o Swagger
builder.Services.AddSwaggerGen(c => // Adiciona o Swagger para documentação da API
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

        
});


    
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy => 
    {
        policy.AllowAnyOrigin() 
              .AllowAnyMethod() 
              .AllowAnyHeader(); 
    });
}); 
   
var app = builder.Build(); 

   
if (app.Environment.IsDevelopment()) 
{
    app.UseSwagger(); 
    app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Beneficiários v1"); 
            c.RoutePrefix = string.Empty; 
            c.DocumentTitle = " Api Beneficiarios - Documentação "; 
        });
}

// O app. Alguma coisa é a instância do WebApplication que representa a aplicação web em execução. Ele é criado a partir do 
// builder e é usado

// para configurar a pipeline de requisições, middlewares e rotas da aplicação antes de iniciar a execução do servidor web. 
// Aqui, estamos adicionando middlewares e configurando a pipeline de requisições da aplicação.

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseSerilogRequestLogging();
app.UseCors("AllowAll"); 
app.UseHttpsRedirection(); 
app.UseAuthorization(); 
app.MapControllers(); 
app.MapHealthChecks("/health");  


app.Logger.LogInformation("🚀 Api beneficiarios iniciada!"); 
app.Logger.LogInformation($"📊 Ambiente: {app.Environment.EnvironmentName}"); 
app.Logger.LogInformation($"🗄️ Banco de dados{(connectionString.Contains("localhost") ? "local (Dokcer)" : "(Produção)")}");


if (app.Environment.IsDevelopment())
{
    app.Logger.LogInformation("📖 Swagger disponível em: http://localhost:5000/");
}


// Abaixo Inicia a execução do servidor web e começa a escutar as requisições HTTP. A aplicação estará pronta para receber e 
// processar as requisições dos clientes.
app.Run(); 

