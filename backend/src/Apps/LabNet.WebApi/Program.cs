using System.Text;
using LabNet.Auth;
using LabNet.Auth.Application.Interfaces;
using LabNet.Auth.Infrastructure.Persistence;
using LabNet.Projects;
using LabNet.Projects.Infrastructure.Persistence;
using LabNet.Shared.Domain.Interfaces;
using LabNet.Shared.Infrastructure.Services;
using LabNet.WebApi.Middlewares;
using LabNet.WebApi.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Controllers & Endpoints
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// SQL Server & DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=localhost,1433;Database=LabNetDb;User Id=sa;Password=Your_Strong_Password123!;TrustServerCertificate=True;MultipleActiveResultSets=true;";

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

// Shared Infrastructure
builder.Services.AddScoped<ICurrentTenantService, CurrentTenantService>();
builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());
builder.Services.AddScoped<IAuthDbContext>(sp => sp.GetRequiredService<AppDbContext>());
builder.Services.AddScoped<IProjectsDbContext>(sp => sp.GetRequiredService<AppDbContext>());

// Bounded Contexts (CodelyTV Modular Architecture)
builder.Services.AddAuthContext();
builder.Services.AddProjectsContext();

// JWT Authentication
var secretKey = builder.Configuration["Jwt:SecretKey"] ?? "super_secure_jwt_secret_key_minimum_32_characters_long_lab_net_2026!";
var issuer = builder.Configuration["Jwt:Issuer"] ?? "LabNet";
var audience = builder.Configuration["Jwt:Audience"] ?? "LabNetClient";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ValidateIssuer = true,
        ValidIssuer = issuer,
        ValidateAudience = true,
        ValidAudience = audience,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Swagger / OpenAPI with Bearer Authorization
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "LabNet SaaS API (CodelyTV Modular DDD Architecture)",
        Version = "v1",
        Description = "Modular Bounded Contexts Architecture with ASP.NET Core & EF Core"
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

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "LabNet SaaS API v1");
    c.RoutePrefix = string.Empty;
});

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseMiddleware<TenantResolutionMiddleware>();
app.UseAuthorization();

app.MapControllers();

// Auto-seed on startup
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
        logger.LogWarning("Database initialization skipped or pending SQL Server ready state: {Message}", ex.Message);
    }
}

app.Run();

public partial class Program { }
