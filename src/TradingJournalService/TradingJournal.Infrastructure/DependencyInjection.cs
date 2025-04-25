using BuildingBlocks.Services;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TradingJournal.Infrastructure.Data.Interceptors;
using TradingJournal.Infrastructure.Data.Repositories;

namespace TradingJournal.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices
        (this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database");

        // Add services to the container.
        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        //services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseNpgsql(connectionString);
        });

        services.AddScoped<ITradingPlanRepository, TradingPlanRepository>();
        services.AddScoped<ITradingTechnicRepository, TradingTechnicRepository>();
        services.AddScoped<ITradeRepository, TradeRepository>();
        services.AddScoped<ITradeAnalyseRepository, TradeAnalyseRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        
        services.AddAutoMapper(typeof(DependencyInjection));
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentSessionProvider, CurrentSessionProvider>();
        return services;
    }
}
