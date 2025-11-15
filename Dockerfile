# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["src/Api.Beneficiario.Domain/Api.Beneficiario.Domain.csproj", "Api.Beneficiario.Domain/"]
COPY ["src/Api.Beneficiario.Application/Api.Beneficiario.Application.csproj", "Api.Beneficiario.Application/"]
COPY ["src/Api.Beneficiario.Infrastructure/Api.Beneficiario.Infrastructure.csproj", "Api.Beneficiario.Infrastructure/"]
COPY ["src/Api.Beneficiario.WebAPI/Api.Beneficiario.WebAPI.csproj", "Api.Beneficiario.WebAPI/"]
RUN dotnet restore "Api.Beneficiario.WebAPI/Api.Beneficiario.WebAPI.csproj"

# Copy everything else and build
COPY src/. .
WORKDIR /src/Api.Beneficiario.WebAPI
RUN dotnet build "Api.Beneficiario.WebAPI.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "Api.Beneficiario.WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

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

ENTRYPOINT ["dotnet", "Api.Beneficiario.WebAPI.dll"]
