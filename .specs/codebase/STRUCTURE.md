# Project Structure

**Root:** `api_beneficiario_net9/`

## Directory Tree

```
api_beneficiario_net9/
├── .dockerignore
├── .env / .env.example
├── .gitignore
├── Dockerfile / Dockerfile.dev
├── docker-compose.yml / docker-compose.prod.yml
├── Makefile
├── setup.sh
├── api_beneficiario_net9.sln
├── README.md
├── src/
│   ├── Api.Beneficiarios.Domain/
│   │   ├── Entities/ (BaseEntity, Beneficiario, Plano)
│   │   ├── Enums/ (StatusBeneficiario)
│   │   └── Interfaces/ (IBeneficiarioRepository, IPlanoRepository)
│   ├── Api.Beneficiarios.Application/
│   │   ├── DTOs/
│   │   │   ├── Beneficiario/ (Create, Update, Response)
│   │   │   ├── Plano/ (Create, Update, Response)
│   │   │   └── Common/ (ErrorResponse)
│   │   └── Services/
│   │       ├── Interfaces/ (IBeneficiarioService, IPlanoService)
│   │       ├── BeneficiarioService.cs
│   │       └── PlanoService.cs
│   ├── Api.Beneficiarios.Infrastructure/
│   │   ├── Configurations/ (EF Fluent API configs)
│   │   ├── Data/ (AppDbContext)
│   │   ├── Migrations/
│   │   └── Repositories/ (BeneficiarioRepository, PlanoRepository)
│   └── Api.Beneficiarios.WebAPI/
│       ├── Controllers/ (BeneficiarioController, PlanoController)
│       ├── Properties/ (launchSettings.json)
│       ├── Program.cs
│       └── appsettings.json / appsettings.Development.json
└── tests/
    └── Api.Beneficiarios.Tests.Unit/
        ├── Application/Services/ (Service tests)
        └── Domain/Validators/ (empty)
```

## Where Things Live

**Beneficiario entity:**
- Domain: `src/Api.Beneficiarios.Domain/Entities/Beneficiario.cs`
- Repository interface: `src/Api.Beneficiarios.Domain/Interfaces/IBeneficiarioRepository.cs`
- Repository impl: `src/Api.Beneficiarios.Infrastructure/Repositories/BeneficiarioRepository.cs`
- Service interface: `src/Api.Beneficiarios.Application/Services/Interfaces/IBeneficiarioService.cs`
- Service impl: `src/Api.Beneficiarios.Application/Services/BeneficiarioService.cs`
- DTOs: `src/Api.Beneficiarios.Application/DTOs/Beneficiario/`
- Controller: `src/Api.Beneficiarios.WebAPI/Controllers/BeneficiarioController.cs`
- EF Config: `src/Api.Beneficiarios.Infrastructure/Configurations/BeneficiarioConfiguration.cs`

**Plano entity:**
- Same structure as Beneficiario, parallel files

**Configuration:**
- EF Core: `src/Api.Beneficiarios.Infrastructure/Configurations/`
- App settings: `src/Api.Beneficiarios.WebAPI/appsettings.json`
- DI wiring: `src/Api.Beneficiarios.WebAPI/Program.cs`
