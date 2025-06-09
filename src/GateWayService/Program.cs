var builder = WebApplication.CreateBuilder(args);


builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//     .AddJwtBearer(options =>
//     {
//         options.Authority = builder.Configuration["IdentityServiceUrl"];
//         options.RequireHttpsMetadata = false;
//         options.TokenValidationParameters.ValidateAudience = false;
//         options.TokenValidationParameters.NameClaimType = "username";
//     });
var clientApp = builder.Configuration["ClientApp"];
Console.WriteLine(clientApp); // بررسی کنید که چه مقداری چاپ می‌شود
var allowedOrigins = System.Text.Json.JsonSerializer.Deserialize<string[]>(clientApp!);
builder.Services.AddCors(options =>
{
    options.AddPolicy("customPolicy", b =>
    {
        b.AllowAnyHeader().AllowAnyMethod().AllowCredentials()
            .WithOrigins(allowedOrigins!);
    });
});

var app = builder.Build();

app.UseCors("customPolicy");

app.MapReverseProxy();

//app.UseAuthentication();
//app.UseAuthorization();

app.Run();
