using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace TradingJournal.Infrastructure.Data.Extensions;
public static class DatabaseExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        //await context.Database.EnsureDeletedAsync();
        await context.Database.MigrateAsync();
        var databaseName = context.Database.GetDbConnection().Database;
        
        if (!databaseName.Contains("Test"))
        {
            //await SeedAsync(context);
        }
    }
    
    // private static async Task SeedAsync(ApplicationDbContext context)
    // {
    //     await SeedTradingPlanAsync(context);
    // }
    //
    //
    // private static async Task SeedTradingPlanAsync(ApplicationDbContext context)
    // {
    //     if (!await context.TradingPlans.AnyAsync())
    //     {
    //         await context.TradingPlans.AddRangeAsync(InitialData.TradingPlans);
    //         await context.SaveChangesAsync();
    //     }
    // }
    
}
