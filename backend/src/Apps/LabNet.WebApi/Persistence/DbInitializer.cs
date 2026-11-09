using LabNet.Auth.Application.Interfaces;
using LabNet.Auth.Domain;
using LabNet.Projects.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LabNet.WebApi.Persistence;

public static class DbInitializer
{
    public static async Task InitializeAsync(
        AppDbContext context,
        IPasswordHasher passwordHasher,
        ILogger logger)
    {
        try
        {
            logger.LogInformation("Ensuring database schema exists...");
            await context.Database.EnsureCreatedAsync();

            if (await context.Tenants.AnyAsync())
            {
                logger.LogInformation("Database already seeded. Skipping initial seed.");
                return;
            }

            logger.LogInformation("Seeding initial tenants, users, and projects...");

            // 1. Seed Tenants
            var acme = new Tenant("Acme Corporation", "acme");
            var globex = new Tenant("Globex Corp", "globex");

            await context.Tenants.AddRangeAsync(acme, globex);
            await context.SaveChangesAsync();

            var passwordHash = passwordHasher.Hash("Password123!");

            // 2. Seed Users
            var acmeAdmin = new User(acme.Id, "admin@acme.com", passwordHash, "Alice Walker", "Admin");
            var acmeMember = new User(acme.Id, "dev@acme.com", passwordHash, "Bob Martin", "Member");

            var globexAdmin = new User(globex.Id, "admin@globex.com", passwordHash, "Hank Scorpio", "Admin");
            var globexMember = new User(globex.Id, "dev@globex.com", passwordHash, "Homer Simpson", "Member");

            await context.Users.AddRangeAsync(acmeAdmin, acmeMember, globexAdmin, globexMember);
            await context.SaveChangesAsync();

            // 3. Seed Projects & Tasks for Acme
            var acmeProject1 = new Project(acme.Id, "CRM Replatforming", "Migration from legacy CRM to modern microservices.");
            acmeProject1.AddTask("Design Database Schema", "Define tables, indices and foreign keys in SQL Server", TaskPriority.High);
            var acmeTask2 = acmeProject1.AddTask("Setup CI/CD Pipeline", "Docker multi-stage builds and GitHub Actions", TaskPriority.Medium);
            acmeTask2.Complete();

            var acmeProject2 = new Project(acme.Id, "Customer Portal Vue 3", "Frontend overhaul using Vite and TypeScript.");
            acmeProject2.AddTask("Implement Modular Architecture", "Follow CodelyTV patterns and Bounded Contexts", TaskPriority.High);

            // 4. Seed Projects & Tasks for Globex
            var globexProject = new Project(globex.Id, "Project Vulcan", "Confidential global expansion infrastructure.");
            globexProject.AddTask("Procure Cloud Hardware", "Provision high performance clusters", TaskPriority.High);
            globexProject.AddTask("Security Audit", "Penetration testing and secret key rotation", TaskPriority.Medium);

            await context.Projects.AddRangeAsync(acmeProject1, acmeProject2, globexProject);
            await context.SaveChangesAsync();

            logger.LogInformation("Database seeded successfully with Acme and Globex tenants!");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }
}
