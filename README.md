# LabNet - Enterprise Multi-Tenant SaaS Showcase

[![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Vue 3](https://img.shields.io/badge/Vue.js-3.x-4FC08D?logo=vuedotjs)](https://vuejs.org/)
[![TypeScript](https://img.shields.io/badge/TypeScript-5.x-3178C6?logo=typescript)](https://www.typescriptlang.org/)
[![SQL Server 2022](https://img.shields.io/badge/SQL_Server-2022-CC292B?logo=microsoftsqlserver)](https://www.microsoft.com/sql-server)
[![Docker](https://img.shields.io/badge/Docker-Compose-2496ED?logo=docker)](https://www.docker.com/)
[![Architecture](https://img.shields.io/badge/Architecture-Modular_DDD_%2F_CodelyTV-orange)](https://codely.com/)

Enterprise-grade SaaS showcase applying **Modular Domain-Driven Design (DDD) & Screaming Architecture (CodelyTV style)**, **Single Action Controllers (REPR Pattern)**, **Database Multi-Tenancy Isolation**, and **End-to-End Type Safety** across **.NET (ASP.NET Core + EF Core)** and **Vue 3 + TypeScript**.

Fully dockerized with **Microsoft SQL Server 2022**.


---

## 🏛️ Arquitectura del Sistema (CodelyTV Modular DDD)

Tanto el Backend como el Frontend siguen el **mismo patrón simétrico de Bounded Contexts**:

### Backend (.NET / ASP.NET Core)
```text
backend/src/
├── Contexts/
│   ├── Shared/
│   │   ├── Domain/           # Entity base, IMustHaveTenant, Excepciones, IUnitOfWork
│   │   └── Infrastructure/   # CurrentTenantService
│   │
│   ├── Auth/
│   │   ├── Domain/           # Tenant, User, ITenantRepository, IUserRepository
│   │   ├── Application/      # LoginUseCase, GetTenantsUseCase, DTOs, Contratos
│   │   └── Infrastructure/   # BCrypt PasswordHasher, JwtTokenGenerator, Repositorios
│   │
│   └── Projects/
│       ├── Domain/           # Project, TaskItem, IProjectRepository
│       ├── Application/      # GetProjectsUseCase, CreateProjectUseCase, AddTask, Toggle
│       └── Infrastructure/   # ProjectRepository (EF Core)
│
└── Apps/
    └── LabNet.WebApi/        # Punto de entrada HTTP
        ├── Controllers/      # Single Action Controllers (un controller por acción HTTP)
        │   ├── Auth/         # LoginPostController, TenantsGetController, MeGetController
        │   └── Projects/     # ProjectsGetController, ProjectGetByIdController, etc.
        ├── Middlewares/      # ExceptionHandlingMiddleware (RFC 7807), TenantResolutionMiddleware
        └── Persistence/      # AppDbContext (EF Core con Global Query Filters) y DbInitializer (Seeders)
```

### Frontend (Vue 3 + TypeScript)
```text
frontend/src/
├── modules/
│   ├── auth/
│   │   ├── domain/         # User (Entity rica), Tenant, AuthRepository (Interface)
│   │   ├── application/    # LoginUseCase, GetTenantsUseCase
│   │   ├── infrastructure/ # HttpAuthRepository (Axios)
│   │   └── ui/             # LoginView.vue, useAuthStore.ts (Pinia)
│   │
│   └── projects/
│       ├── domain/         # Project, TaskItem, ProjectRepository (Interface)
│       ├── application/    # ProjectUseCases (Get, Create, AddTask, Toggle)
│       ├── infrastructure/ # HttpProjectRepository
│       └── ui/             # ProjectsDashboardView.vue, useProjectsStore.ts
│
└── shared/
    └── infrastructure/http/ # httpClient con interceptores (Bearer Token + X-Tenant-Id)
```

---

## 🔐 Seguridad y Multi-Tenancy

1. **Aislamiento Multi-Tenant**: Cada entidad sensible implementa `IMustHaveTenant`. En `AppDbContext`:
   ```csharp
   builder.Entity<Project>().HasQueryFilter(p => !_currentTenantService.HasTenant || p.TenantId == _currentTenantService.TenantId);
   ```
2. **Extracción Automática**: El `TenantResolutionMiddleware` inspecciona las claims del token JWT verificado (`tenant_id`) o el encabezado `X-Tenant-Id`.
3. **Manejo Centralizado de Excepciones**: Respuestas estandarizadas RFC 7807 (`ProblemDetails`) con códigos HTTP semánticos (400 para reglas de dominio, 404 para entidades inexistentes, 401 para accesos no autorizados).

---

## 🚀 Despliegue con Docker (Recomendado)

```bash
docker compose up --build
```

Servicios:
- **Frontend SPA**: [http://localhost:5173](http://localhost:5173)
- **Backend API & Swagger**: [http://localhost:5000](http://localhost:5000)
- **Microsoft SQL Server 2022**: `localhost:1433` (usuario `sa`, password `Your_Strong_Password123!`)

---

## 👤 Credenciales Pre-Cargadas (Seeders)

Contraseña para todos los usuarios: `Password123!`

### Tenant 1: Acme Corporation (`acme`)
- **Admin**: `admin@acme.com`
- **Member**: `dev@acme.com`

### Tenant 2: Globex Corp (`globex`)
- **Admin**: `admin@globex.com`
- **Member**: `dev@globex.com`

---

## 🧪 Ejecución de Tests Unitarios

### Tests de Backend (.NET xUnit + FluentAssertions + Moq)
```bash
dotnet test backend/tests/LabNet.UnitTests/LabNet.UnitTests.csproj
```

### Tests de Frontend (Vitest + @vue/test-utils)
```bash
cd frontend
npx vitest run
```
