# LabNet - SaaS Multi-Tenant Showcase (CodelyTV Modular DDD Architecture)

Enterprise-grade MVP demonstrating **Modular Bounded Contexts (CodelyTV style)**, **Domain-Driven Design (DDD)**, **Multi-Tenancy Isolation**, and **End-to-End Type Safety** across **.NET (ASP.NET Core + EF Core)** and **Vue 3 + TypeScript**.

Fully dockerized with **Microsoft SQL Server 2022**.

---

## 📋 Guía de Respuestas para la Entrevista (Para Juan)

Copiá o adaptá estos puntos para responder el correo técnico con total autoridad y transparencia:

### 1. ¿Cómo describirías tu nivel actual de inglés conversacional?
> *(Completá con tu nivel real, ej: B2/C1 profesional, capaz de mantener reuniones técnicas de diseño y standups sin inconvenientes).*

### 2. ¿Tenés experiencia con C#/.NET y ASP.NET Core? ¿En qué proyectos y qué tareas realizaste?
> *"Vengo con un sólido background en desarrollo backend con tipado estricto y diseño empresarial (DDD, Bounded Contexts, Hexagonal Architecture, SOLID). La curva hacia C# y .NET es inmediata porque los conceptos de modelado de dominio y desacoplamiento son agnósticos al lenguaje.*
>
> *Para demostrarlo con código en mano, construí un MVP funcional (**LabNet**) estructurado bajo la arquitectura modular por Bounded Contexts de CodelyTV (Contexts: Shared, Auth, Projects; Apps: WebApi). Cada contexto encapsula su propio Domain, Application e Infrastructure, con middlewares para excepciones (RFC 7807), resolución multi-tenant, seguridad JWT y tests unitarios con xUnit, FluentAssertions y Moq."*

### 3. ¿Trabajaste con Vue.js y TypeScript juntos?
> *"Sí. En el frontend trabajo con Vue 3 (Composition API) junto a TypeScript estricto, aplicando la misma arquitectura modular por Bounded Contexts / Features (prácticas CodelyTV). Separo la capa de dominio/entidades, casos de uso (Application), repositorios HTTP (Infrastructure) y componentes UI/Pinia. El proyecto incluye suite de tests unitarios ejecutados con Vitest y @vue/test-utils."*

### 4. ¿Qué experiencia tenés con SQL Server y Entity Framework Core?
> *"Utilizo Entity Framework Core con enfoque Code-First conectado a SQL Server en contenedores Docker. En el proyecto implementé modelado relacional, Fluent API, Change Tracking, Unit of Work y **Global Query Filters** (`HasQueryFilter`) para aislamiento automático de datos multi-tenant a nivel de consulta LINQ, garantizando que ninguna consulta filtre datos entre organizaciones distintas. Además, cuenta con inicializadores/seeders automáticos con hash seguro de contraseñas (BCrypt)."*

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
    └── LabNet.WebApi/        # Punto de entrada HTTP (Controllers, Middlewares, AppDbContext)
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
