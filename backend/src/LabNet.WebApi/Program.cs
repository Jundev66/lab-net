using LabNet.Application;
using LabNet.Application.Common.Interfaces;
using LabNet.Infrastructure;
using LabNet.Infrastructure.Persistence;
using LabNet.WebApi.Middlewares;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Application & Infrastructure Layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// CORS for Frontend (Vue 3)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Swagger / OpenAPI with JWT Bearer Support
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "LabNet SaaS API",
        Version = "v1",
        Description = "Enterprise Clean Architecture & Multitenancy MVP with ASP.NET Core & EF Core"
    });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter JWT Bearer token: Bearer {token}",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Id = "Bearer",
            Type = ReferenceType.SecurityScheme
        }
    };

    c.AddSecurityDefinition("Bearer", securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, Array.Empty<string>() }
    });
});

var app = builder.Build();

// Middlewares pipeline
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "LabNet SaaS API v1");
    c.RoutePrefix = string.Empty; // Serve Swagger at root: http://localhost:5000/
});

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseMiddleware<TenantResolutionMiddleware>();
app.UseAuthorization();

app.MapControllers();

// Auto-seed database during startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    var dbContext = services.GetRequiredService<AppDbContext>();
    var passwordHasher = services.GetRequiredService<IPasswordHasher>();

    try
    {
        await DbInitializer.InitializeAsync(dbContext, passwordHasher, logger);
    }
    catch (Exception ex)
    {
        logger.LogWarning("Database initialization skipped or failed (SQL Server might be booting up in Docker): {Message}", ex.Message);
    }
}

app.Run();

// For integration tests
public partial class Program { }
