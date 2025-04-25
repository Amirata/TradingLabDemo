using System.Text.Json;
using AuthService.Models;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure RefreshToken
        builder.Entity<RefreshToken>()
            .HasKey(r => r.Id);

        builder.Entity<RefreshToken>()
            .HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.AddInboxStateEntity();
        builder.AddOutboxMessageEntity();
        builder.AddOutboxStateEntity();
        
        
        SeedData(builder);
    }
    
    private void SeedData(ModelBuilder modelBuilder)
    {
        
        var aspNetRolesFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Data/JsonFiles", "AspNetRoles.json");
        var aspNetUserRolesFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Data/JsonFiles", "AspNetUserRoles.json");
        var aspNetUsersFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Data/JsonFiles", "AspNetUsers.json");
        
        if (File.Exists(aspNetRolesFilePath))
        {
            var jsonData = File.ReadAllText(aspNetRolesFilePath);
            
            var identityRoles = JsonSerializer.Deserialize<List<IdentityRole>>(jsonData);

            if (identityRoles != null)
            {
                modelBuilder.Entity<IdentityRole>().HasData(identityRoles);
            }
        }
        
        if (File.Exists(aspNetUsersFilePath))
        {
           
            var jsonData = File.ReadAllText(aspNetUsersFilePath);
            
            var users = JsonSerializer.Deserialize<List<ApplicationUser>>(jsonData);

            if (users != null)
            {
                modelBuilder.Entity<ApplicationUser>().HasData(users);
            }
        }
        
        if (File.Exists(aspNetUserRolesFilePath))
        {
            var jsonData = File.ReadAllText(aspNetUserRolesFilePath);
            
            var identityUserRoles = JsonSerializer.Deserialize<List<IdentityUserRole<string>>>(jsonData);

            if (identityUserRoles != null)
            {
                modelBuilder.Entity<IdentityUserRole<string>>().HasData(identityUserRoles);
            }
        }
    }
}