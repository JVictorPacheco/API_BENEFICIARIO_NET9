using Microsoft.EntityFrameworkCore;
using Api.Beneficiarios.Infrastructure.Data;
using Api.Beneficiarios.Infrastructure.Repositories;
using Api.Beneficiarios.Domain.Interfaces;
using Api.Beneficiarios.Application.Services;
using Api.Beneficiarios.Application.Services.Interfaces;


var builder = WebApplication.CreateBuilder(args); 


var connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING")  
                       ?? builder.Configuration.GetConnectionString("DefaultConnection"); 


if (string.IsNullOrEmpty(connectionString)) 
{
    throw new InvalidOperationException("Connection string não configurada! Verifique appsettings.json ou variável de ambiente DATABASE_CONNECTION_STRING"); 
}



builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(connectionString);

    
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging(); 
        options.EnableDetailedErrors(); 
    }
});


builder.Services.AddScoped<IBeneficiarioRepository, BeneficiarioRepository>(); 
builder.Services.AddScoped<IPlanoRepository, PlanoRepository>();

builder.Services.AddScoped<IBeneficiarioService, BeneficiarioService>();
builder.Services.AddScoped<IPlanoService, PlanoService>();



builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter()
        );

        options.JsonSerializerOptions.DefaultIgnoreCondition =
            System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });



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


app.UseCors("AllowAll"); 
app.UseHttpsRedirection(); 
app.UseAuthorization(); 
app.MapControllers(); 



app.Logger.LogInformation("🚀 Api beneficiarios iniciada!"); 
app.Logger.LogInformation($"📊 Ambiente: {app.Environment.EnvironmentName}"); 
app.Logger.LogInformation($"🗄️ Banco de dados{(connectionString.Contains("localhost") ? "local (Dokcer)" : "(Produção)")}");


if (app.Environment.IsDevelopment())
{
    app.Logger.LogInformation("📖 Swagger disponível em: http://localhost:5000/");
}


app.Run(); 

