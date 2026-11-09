# LabNet - SaaS Multi-Tenant Showcase

Enterprise-grade MVP demonstrating **Clean / Hexagonal Architecture**, **Domain-Driven Design (DDD)**, **Multi-Tenancy Isolation**, and **End-to-End Type Safety** across **.NET (ASP.NET Core + EF Core)** and **Vue 3 + TypeScript**.

Fully dockerized with **Microsoft SQL Server 2022**.

---

## 📋 Guía de Respuestas para la Entrevista (Para Juan)

Copiá o adaptá estos puntos para responder el correo técnico con total autoridad y transparencia:

### 1. ¿Cómo describirías tu nivel actual de inglés conversacional?
> *(Completá con tu nivel real, ej: B2/C1 profesional, capaz de mantener reuniones técnicas de diseño y standups sin inconvenientes).*

### 2. ¿Tenés experiencia con C#/.NET y ASP.NET Core? ¿En qué proyectos y qué tareas realizaste?
> *"Vengo con un sólido background en desarrollo backend con lenguajes fuertemente tipados y arquitecturas empresariales (DDD, Hexagonal / Clean Architecture, SOLID). Si bien mi experiencia previa laboral se concentró en otros stacks de backend, la transición a C# y .NET es inmediata porque las bases arquitectónicas, patrones de diseño y modelado de dominio son agnósticos al framework.*
>
> *Para demostrarlo concretamente, construí un MVP funcional (**LabNet**) implementando una API REST en ASP.NET Core con Clean Architecture desacoplada (Domain, Application, Infrastructure, WebApi), Middlewares personalizados para manejo global de excepciones (RFC 7807) y resolución de tenants, autenticación JWT Bearer, y tests unitarios con xUnit, FluentAssertions y Moq."*

### 3. ¿Trabajaste con Vue.js y TypeScript juntos?
> *"Sí. En el frontend trabajo con Vue 3 (Composition API) junto a TypeScript estricto, aplicando arquitectura modular por Bounded Contexts / Features (siguiendo las prácticas de CodelyTV). Separo la capa de dominio/entidades, casos de uso (Application), repositorios HTTP (Infrastructure) y componentes UI/Pinia. El proyecto incluye suite de tests unitarios ejecutados con Vitest y @vue/test-utils."*

### 4. ¿Qué experiencia tenés con SQL Server y Entity Framework Core?
> *"Utilizo Entity Framework Core con enfoque Code-First conectado a SQL Server en contenedores Docker. En el proyecto implementé modelado relacional, Fluent API, Change Tracking, Unit of Work y **Global Query Filters** (`HasQueryFilter`) para aislamiento automático de datos multi-tenant a nivel de consulta LINQ, garantizando que ninguna consulta filtre datos entre organizaciones distintas. Además, cuenta con inicializadores/seeders automáticos con hash seguro de contraseñas (BCrypt)."*

---

## 🏛️ Arquitectura del Sistema

### Backend (.NET / ASP.NET Core) — Hexagonal Architecture
```text
backend/src/
├── LabNet.Domain/          # Núcleo puro (sin dependencias externas)
│   ├── Common/             # Entity base, IMustHaveTenant, Excepciones de dominio
│   ├── Entities/           # Tenant, User, Project, TaskItem
│   └── Interfaces/         # Ports: IUserRepository, IProjectRepository, IUnitOfWork, ICurrentTenantService
│
├── LabNet.Application/     # Casos de uso de la aplicación
│   ├── Auth/               # LoginUseCase, GetTenantsUseCase, DTOs
│   ├── Projects/           # GetProjectsUseCase, CreateProjectUseCase, AddTaskUseCase, ToggleTaskUseCase
│   └── Common/Interfaces/  # IJwtTokenGenerator, IPasswordHasher
│
├── LabNet.Infrastructure/  # Adaptadores secundarios (I/O & Persistencia)
│   ├── Persistence/        # AppDbContext (EF Core), Mapeos Fluent API, Global Query Filters
│   ├── Repositories/       # Implementación de repositorios SQL Server
│   ├── Services/           # BCrypt PasswordHasher, JwtTokenGenerator, CurrentTenantService
│   └── Persistence/        # DbInitializer (Seeders automáticos)
│
└── LabNet.WebApi/          # Adaptadores primarios (HTTP Presentation)
    ├── Controllers/        # AuthController, ProjectsController
    ├── Middlewares/        # ExceptionHandlingMiddleware (RFC 7807), TenantResolutionMiddleware
    └── Program.cs          # Pipeline HTTP, CORS, Swagger UI con Bearer JWT
```

### Frontend (Vue 3 + TypeScript) — Modular por Features (CodelyTV)
```text
frontend/src/
├── modules/
│   ├── auth/
│   │   ├── domain/         # User (Entity rica), Tenant, AuthRepository (Interface)
│   │   ├── infrastructure/ # HttpAuthRepository (Axios)
│   │   ├── application/    # LoginUseCase, GetTenantsUseCase
│   │   └── ui/             # LoginView.vue, useAuthStore.ts (Pinia)
│   │
│   └── projects/
│       ├── domain/         # Project, TaskItem, ProjectRepository (Interface)
│       ├── infrastructure/ # HttpProjectRepository
│       ├── application/    # ProjectUseCases (Get, Create, AddTask, Toggle)
│       └── ui/             # ProjectsDashboardView.vue, useProjectsStore.ts
│
└── shared/
    └── infrastructure/http/ # httpClient con interceptores (Bearer Token + X-Tenant-Id)
```

---

## 🔐 Seguridad y Multi-Tenancy

1. **Aislamiento Multi-Tenant**: Cada registro sensible (`User`, `Project`, `TaskItem`) implementa `IMustHaveTenant`. En `AppDbContext`:
   ```csharp
   builder.Entity<Project>().HasQueryFilter(p => !_currentTenantService.HasTenant || p.TenantId == _currentTenantService.TenantId);
   ```
2. **Extracción Automática**: El `TenantResolutionMiddleware` inspecciona las claims del token JWT verificado (`tenant_id`) o el encabezado `X-Tenant-Id`.
3. **Manejo Centralizado de Excepciones**: Respuestas estandarizadas RFC 7807 (`ProblemDetails`) con códigos HTTP semánticos (400 para reglas de dominio, 404 para entidades inexistentes, 401 para accesos no autorizados).

---

## 🚀 Despliegue con Docker (Recomendado)

Todo el stack se orquesta con un único comando:

```bash
docker compose up --build
```

Servicios levantados:
- **Frontend SPA**: [http://localhost:5173](http://localhost:5173)
- **Backend API & Swagger**: [http://localhost:5000](http://localhost:5000)
- **Microsoft SQL Server 2022**: `localhost:1433` (usuario `sa`, password `Your_Strong_Password123!`)

---

## 👤 Credenciales Pre-Cargadas (Seeders)

El sistema genera automáticamente dos organizaciones (Tenants) aisladas al iniciar:

### Tenant 1: Acme Corporation (`acme`)
| Rol | Email | Password |
|---|---|---|
| **Admin** | `admin@acme.com` | `Password123!` |
| **Member** | `dev@acme.com` | `Password123!` |

### Tenant 2: Globex Corp (`globex`)
| Rol | Email | Password |
|---|---|---|
| **Admin** | `admin@globex.com` | `Password123!` |
| **Member** | `dev@globex.com` | `Password123!` |

*Prueba de aislamiento:* Ingresá con `admin@acme.com` y verás los proyectos de Acme. Cerrá sesión e ingresá con `admin@globex.com`: verás **únicamente** los proyectos confidenciales de Globex.

---

## 🧪 Ejecución de Tests Unitarios

### Tests de Backend (.NET xUnit + FluentAssertions + Moq)
```bash
dotnet test backend/tests/LabNet.UnitTests/LabNet.UnitTests.csproj
```
*(15 tests unitarios verificando invariantes de dominio, hashing y casos de uso con mocks).*

### Tests de Frontend (Vitest + @vue/test-utils)
```bash
cd frontend
npx vitest run
```
*(6 tests unitarios verificando entidades de dominio de TypeScript, métodos calculados y casos de uso con mocks).*
