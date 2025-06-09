using Scalar.AspNetCore;
using TradingJournal.Api;
using TradingJournal.Application;
using TradingJournal.Infrastructure;
using TradingJournal.Infrastructure.Data.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplicationServices(builder.Configuration)
    .AddInfrastructureServices(builder.Configuration)
    .AddApiServices(builder.Configuration);


var app = builder.Build();

// فعال‌سازی CORS
app.UseCors("MyCorsPolicy");

app.UseStaticFiles();

app.UseExceptionHandler(opt=>{});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options => { options.RouteTemplate = "/openapi/{documentName}.json"; options.SerializeAsV2 = true; });
    app.MapScalarApiReference();
    await app.InitialiseDatabaseAsync();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();