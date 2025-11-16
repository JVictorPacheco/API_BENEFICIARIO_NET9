# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["src/Api.Beneficiario.Domain/Api.Beneficiarios.Domain.csproj", "Api.Beneficiarios.Domain/"]
COPY ["src/Api.Beneficiario.Application/Api.Beneficiarios.Application.csproj", "Api.Beneficiarios.Application/"]
COPY ["src/Api.Beneficiario.Infrastructure/Api.Beneficiarios.Infrastructure.csproj", "Api.Beneficiarios.Infrastructure/"]
COPY ["src/Api.Beneficiario.WebAPI/Api.Beneficiarios.WebAPI.csproj", "Api.Beneficiarios.WebAPI/"]
RUN dotnet restore "Api.Beneficiarios.WebAPI/Api.Beneficiarios.WebAPI.csproj"

# Copy everything else and build
COPY src/. .
WORKDIR /src/Api.Beneficiario.WebAPI
RUN dotnet build "Api.Beneficiarios.WebAPI.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "Api.Beneficiarios.WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
EXPOSE 5000

# Create a non-root user
RUN addgroup --system --gid 1000 appuser \
    && adduser --system --uid 1000 --ingroup appuser --shell /bin/sh appuser

# Copy published files
COPY --from=publish /app/publish .

# Change ownership to non-root user
RUN chown -R appuser:appuser /app
USER appuser

ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Development

ENTRYPOINT ["dotnet", "Api.Beneficiarios.WebAPI.dll"]
