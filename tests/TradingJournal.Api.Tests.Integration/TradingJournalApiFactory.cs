using System.Data.Common;
using BuildingBlocks.Services;
using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Testcontainers.PostgreSql;
using TradingJournal.Infrastructure.Data;

namespace TradingJournal.Api.Tests.Integration;

public class TradingJournalApiFactory : WebApplicationFactory<IApiMarker>, IAsyncLifetime
{
    private readonly string _connectionString =
        "Server=127.0.0.1;Port=5434;Database=TradingLabTest;User Id=postgres;Password=postgres;";

    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithName("PostgresTest")
        .WithEnvironment("POSTGRES_USER", "postgres")
        .WithEnvironment("POSTGRES_PASSWORD", "postgres")
        //.WithEnvironment("POSTGRES_DB", "TradingLab")
        .WithPortBinding(5434, 5432)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5432))
        .Build();

    public TestLoggerProvider LoggerProvider { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Add the custom logger
        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();

            // Set the minimum log level
            logging.SetMinimumLevel(LogLevel.Warning);

            // Configure specific log levels for different categories
            logging.AddFilter("Microsoft", LogLevel.None);
            logging.AddFilter("System", LogLevel.None);
            //logging.AddFilter("YourNamespace", LogLevel.Debug);

            logging.AddProvider(LoggerProvider);
        });


        builder.ConfigureTestServices(services =>
        {
            //services.RemoveAll(typeof(IHostedService));

            services.Remove(services.SingleOrDefault(service =>
                typeof(DbContextOptions<ApplicationDbContext>) == service.ServiceType)!);

            services.Remove(services.SingleOrDefault(service => typeof(DbConnection) == service.ServiceType)!);

            services.Remove(services.SingleOrDefault(service => typeof(DbConnection) == service.ServiceType)!);
            
          
            var descriptor = services.SingleOrDefault(service => 
                service.ServiceType == typeof(ICurrentSessionProvider));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }
            
            
            services.AddScoped<ICurrentSessionProvider, TestCurrentSessionProvider>();

            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
                options.UseNpgsql(_connectionString);
            });

            // // Seed the database after it's configured
            // using var serviceProvider = services.BuildServiceProvider();
            // using var scope = serviceProvider.CreateScope();
            // var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            //
            // db.Database.EnsureCreated();
            // SeedDatabase(db);
        });
    }

    // private void SeedDatabase(ApplicationDbContext dbContext)
    // {
    //     // Add your test data here
    //     dbContext.Users.Add(new User { Name = "Test User", Email = "test@example.com" });
    //     dbContext.SaveChanges();
    // }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
        DeleteFolderIfExists();
    }

    private static void DeleteFolderIfExists()
    {
        var folderPath = Path.Combine("wwwroot", "TechnicImages");
        if (Directory.Exists(folderPath))
        {
            Directory.Delete(folderPath, recursive: true);
        }
    }
}