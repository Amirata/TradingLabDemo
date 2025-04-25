using AuthService.Models;
using BuildingBlocks.Auth.Enums;
using Contracts.User;
using MassTransit;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Data;

public static class SeedDataBase
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var publishEndpoint = services.GetRequiredService<IPublishEndpoint>();
        var db = services.GetRequiredService<ApplicationDbContext>();

        // Seed roles
        await EnsureRoleAsync(roleManager, nameof(Roles.User));
        await EnsureRoleAsync(roleManager, nameof(Roles.Admin));

        // Seed users
        await EnsureUserAsync(userManager, "admin", "admin@gmail.com", "Pass123", nameof(Roles.Admin), publishEndpoint,
            db);
    }

    private static async Task EnsureRoleAsync(RoleManager<IdentityRole> roleManager, string roleName)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    private static async Task EnsureUserAsync(UserManager<ApplicationUser> userManager, string name, string email,
        string password, string roleName, IPublishEndpoint publishEndpoint, ApplicationDbContext db)
    {
        if (await userManager.FindByNameAsync(name) == null)
        {
            var user = new ApplicationUser() { UserName = name, Email = email, EmailConfirmed = true };
            var result = await userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, roleName);
                await publishEndpoint.Publish(new UserCreated
                {
                    UserName = user.UserName,
                    Id = Guid.Parse(user.Id)
                });
                await db.SaveChangesAsync();
            }
        }
    }
}