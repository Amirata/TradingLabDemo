using System.Reflection;
using System.Text;
using BuildingBlocks.Exceptions.Handler;
using Microsoft.OpenApi.Models;
using BuildingBlocks.Auth.Enums;
using BuildingBlocks.Services;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using TradingJournal.Api.Consumers;

namespace TradingJournal.Api;
public static class DependencyInjection
{
    public static IServiceCollection AddApiServices
        (this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
            options.ApiVersionReader = ApiVersionReader.Combine(
                new UrlSegmentApiVersionReader(),
                new HeaderApiVersionReader("X-Api-Version")
            );
        }).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.UseAllOfToExtendReferenceSchemas();
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API - V1", Version = "v1.0" });
            
            
            // using System.Reflection;
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });

        services.AddExceptionHandler<CustomExceptionHandler>();
        
        var jwtSettings = configuration.GetSection("Jwt");
        var secretKey = jwtSettings["Key"];
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];

        services.AddAuthentication(options =>
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
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!)),
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    ClockSkew = TimeSpan.Zero,
                    ValidateLifetime = true
                };
            });

        services.AddAuthorizationBuilder()
            .AddPolicy(nameof(Policies.AdminOnly), policy => policy.RequireRole(nameof(Roles.Admin)))
            .AddPolicy(nameof(Policies.AdminOrUser), policy =>
                policy.RequireRole(nameof(Roles.Admin), nameof(Roles.User)));

        
        services.AddMassTransit(x =>
        {
            x.AddConsumersFromNamespaceContaining<UserCreatedConsumer>();

            x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("journal", false));

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(configuration["RabbitMq:Host"], "/", h =>
                {
                    h.Username(configuration.GetValue("RabbitMQ:Username", "guest")!);
                    h.Password(configuration.GetValue("RabbitMQ:Password", "guest")!);
                });

                cfg.ReceiveEndpoint("journal-user-created", e =>
                {
                    e.UseMessageRetry(r => r.Interval(5,5));
            
                    e.ConfigureConsumer<UserCreatedConsumer>(context);
                });

                cfg.ConfigureEndpoints(context);
            });
        });
        
        

        return services;
    }
}
