using LibraryManagement.Web.Models;
using LibraryManagement.Web.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace LibraryManagement.Web.Data.SeedData;

/// <summary>
/// Runs once at application startup (invoked from Program.cs). Applies any
/// pending EF Core migrations, then ensures the "Admin" role and a default
/// Administrator account exist so the app is usable on first run.
/// </summary>
public static class DbInitializer
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("DbInitializer");

        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var appSettings = services.GetRequiredService<IOptions<AppSettings>>().Value;

        // Apply any pending migrations automatically (dev convenience;
        // in production you'd typically run migrations via a separate step).
        await context.Database.MigrateAsync();

        // --- Seed roles ---
        string[] roles = { "Admin", "Member" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
                logger.LogInformation("Created role {Role}", role);
            }
        }

        // --- Seed default Admin user ---
        var adminEmail = appSettings.DefaultAdminEmail;
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser is null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "System Administrator",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, appSettings.DefaultAdminPassword);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
                logger.LogInformation("Seeded default Admin account {Email}", adminEmail);
            }
            else
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                logger.LogError("Failed to seed default Admin account: {Errors}", errors);
            }
        }

        // --- Seed a few lookup rows so dropdowns aren't empty on first run ---
        if (!await context.Categories.AnyAsync())
        {
            context.Categories.AddRange(
                new Category { Name = "Fiction", Description = "Novels and short stories" },
                new Category { Name = "Non-Fiction", Description = "Biographies, essays, factual works" },
                new Category { Name = "Science & Technology", Description = "STEM and computing" }
            );
            await context.SaveChangesAsync();
            logger.LogInformation("Seeded default categories");
        }
    }
}