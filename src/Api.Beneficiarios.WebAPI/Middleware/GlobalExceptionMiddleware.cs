using FluentValidation;
using Api.Beneficiarios.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;


namespace Api.Beneficiarios.WebAPI.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, IWebHostEnvironment env, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _env = env;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }


        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var problem = new ProblemDetails();

            switch (exception)
            {
                case NotFoundException notFoundException:
                    problem.Status = StatusCodes.Status404NotFound;
                    problem.Title = "Recurso não encontrado";
                    problem.Detail = notFoundException.Message;
                    break;

                case ValidationException validationException:
                    problem.Status = StatusCodes.Status400BadRequest;
                    problem.Title = "Erro na validação";
                    problem.Detail = "Um ou mais erros de validação ocorreram.";
                    problem.Extensions["errors"] = validationException.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                    break;

                case BusinessRuleException businessException:
                    problem.Status = StatusCodes.Status409Conflict;
                    problem.Title = "Violação de regra de negócio";
                    problem.Detail = businessException.Message;
                    break;

                default:
                    problem.Status = StatusCodes.Status500InternalServerError;
                    problem.Title = "Erro interno do servidor";
                    problem.Detail = _env.IsDevelopment() ? exception.Message : "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.";
                    break;
            }
        

            if (_env.IsDevelopment())
            {
                 problem.Extensions["traceId"] = context.TraceIdentifier;
                 problem.Extensions["stackTrace"] = exception.StackTrace;
            }

            _logger.LogError(exception, "Ocorreu uma exceção não tratada: {Message}", exception.Message);


            context.Response.StatusCode = problem.Status ?? StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/problem+json";

            await context.Response.WriteAsJsonAsync(problem);
        }
    }
}