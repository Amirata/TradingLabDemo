using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace TradingJournal.Infrastructure.Data;
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
        
        SeedData(builder);
    }

    public DbSet<TradingPlan> TradingPlans => Set<TradingPlan>();
    public DbSet<TradingTechnic> TradingTechnics => Set<TradingTechnic>();
    public DbSet<TradingTechnicImage> TradingTechnicImages => Set<TradingTechnicImage>();
    public DbSet<Trade> Trades => Set<Trade>();
    public DbSet<User> Users => Set<User>();
    
    
    
    private void SeedData(ModelBuilder modelBuilder)
    {
        
        var usersFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Data/JsonFiles", "Users.json");
        var tradingTechnicImagesFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Data/JsonFiles", "TradingTechnicImages.json");
        var tradingTechnicsFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Data/JsonFiles", "TradingTechnics.json");
        var tradingPlansFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Data/JsonFiles", "TradingPlans.json");
        var plansTechnicsFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Data/JsonFiles", "PlansTechnics.json");
        var tradesFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Data/JsonFiles", "Trades.json");
     
        
        if (File.Exists(usersFilePath))
        {
            var jsonData = File.ReadAllText(usersFilePath);
            
            var options = new JsonSerializerOptions
            {
                Converters =
                {
                    new UserIdConverter(),
                    new NullableDateTimeConverter()
                },
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            
            var users = JsonSerializer.Deserialize<List<User>>(jsonData, options);

            if (users != null)
            {
                modelBuilder.Entity<User>().HasData(users);
            }
        }
        
        if (File.Exists(tradingTechnicImagesFilePath))
        {
           
            var jsonData = File.ReadAllText(tradingTechnicImagesFilePath);
            
            var options = new JsonSerializerOptions
            {
                Converters =
                {
                    new TradingTechnicIdConverter(),
                    new NullableDateTimeConverter()
                },
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            
            var tradingTechnicImages = JsonSerializer.Deserialize<List<TradingTechnicImage>>(jsonData,options);
        
            if (tradingTechnicImages != null)
            {
                modelBuilder.Entity<TradingTechnicImage>().HasData(tradingTechnicImages);
            }
        }
        
        if (File.Exists(tradingTechnicsFilePath))
        {
            var jsonData = File.ReadAllText(tradingTechnicsFilePath);
            
            var options = new JsonSerializerOptions
            {
                Converters =
                {
                    new UserIdConverter(),
                    new TradingTechnicIdConverter(),
                    new NullableDateTimeConverter()
                },
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            
            var tradingTechnics = JsonSerializer.Deserialize<List<TradingTechnic>>(jsonData,options);
        
            if (tradingTechnics != null)
            {
                modelBuilder.Entity<TradingTechnic>().HasData(tradingTechnics);
            }
        }
        
        if (File.Exists(tradingPlansFilePath))
        {
            var jsonData = File.ReadAllText(tradingPlansFilePath);
            
            var options = new JsonSerializerOptions
            {
                Converters =
                {
                    new UserIdConverter(),
                    new TradingPlanIdConverter(),
                    new NullableDateTimeConverter()
                },
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            
            var tradingPlans = JsonSerializer.Deserialize<List<TradingPlan>>(jsonData,options);
        
            if (tradingPlans != null)
            {
                modelBuilder.Entity<TradingPlan>().HasData(tradingPlans);
            }
        }
        
        if (File.Exists(plansTechnicsFilePath))
        {
           
                var jsonData = File.ReadAllText(plansTechnicsFilePath);
                
                var plansTechnics = JsonSerializer.Deserialize<List<Dictionary<string, Guid>>>(jsonData);
                if (plansTechnics != null)
                {
                    
                    var plansTechnicsData = plansTechnics.Select((pt, index) =>
                    {
                        //Console.WriteLine($"PlansTechnics: TradingPlansId={pt["TradingPlansId"]}, TechnicsId={pt["TechnicsId"]}");
                        return new
                        {
                            TradingPlansId = TradingPlanId.Of(pt["TradingPlansId"]),
                            TechnicsId = TradingTechnicId.Of(pt["TechnicsId"])
                        };
                    }).ToList();
                    modelBuilder.Entity("PlansTechnics").HasData(plansTechnicsData);
                }
           
        }
        
        if (File.Exists(tradesFilePath))
        {
            var jsonData = File.ReadAllText(tradesFilePath);
            
            var options = new JsonSerializerOptions
            {
                Converters =
                {
                    new TradeIdConverter(),
                    new TradingPlanIdConverter(),
                    new DateTimeConverter()
                },
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            
            var trades = JsonSerializer.Deserialize<List<Trade>>(jsonData,options);
        
            if (trades != null)
            {
                modelBuilder.Entity<Trade>().HasData(trades);
            }
        }
    }
}

public class NullableDateTimeConverter : JsonConverter<DateTime?>
{
    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        if (reader.TokenType != JsonTokenType.String)
            throw new JsonException("Expected a string or null for DateTime?.");

        var dateString = reader.GetString();
        if (string.IsNullOrEmpty(dateString))
            return null;

        // تلاش برای تبدیل فرمت‌های مختلف
        if (DateTime.TryParse(dateString, out var dateTime))
        {
            //Console.WriteLine(dateTime);
            return dateTime;
        }

        throw new JsonException($"Cannot convert '{dateString}' to DateTime?.");
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
            writer.WriteStringValue(value.Value.ToString("O")); // فرمت ISO 8601
        else
            writer.WriteNullValue();
    }
}

public class DateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
            throw new JsonException("Expected a string for DateTime.");

        var dateString = reader.GetString();
        if (string.IsNullOrEmpty(dateString))
            throw new JsonException("Expected a string for DateTime.");

        // تلاش برای تبدیل فرمت‌های مختلف
        if (DateTime.TryParse(dateString, out var dateTime))
        {
            //Console.WriteLine(dateTime);
            return dateTime;
        }

        throw new JsonException($"Cannot convert '{dateString}' to DateTime?.");
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("O")); // فرمت ISO 8601
    }
}

public class UserIdConverter : JsonConverter<UserId>
{
    public override UserId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
            throw new JsonException("Expected a string for UserId.");

        var guidString = reader.GetString();
        if (!Guid.TryParse(guidString, out var guid))
            throw new JsonException("Invalid Guid format for UserId.");

        return UserId.Of(guid);
    }

    public override void Write(Utf8JsonWriter writer, UserId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value.ToString());
    }
}

public class TradingTechnicIdConverter : JsonConverter<TradingTechnicId>
{
    public override TradingTechnicId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
            throw new JsonException("Expected a string for TradingTechnicId.");

        var guidString = reader.GetString();
        if (!Guid.TryParse(guidString, out var guid))
            throw new JsonException("Invalid Guid format for TradingTechnicId.");

        return TradingTechnicId.Of(guid);
    }

    public override void Write(Utf8JsonWriter writer, TradingTechnicId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value.ToString());
    }
}

public class TradingPlanIdConverter : JsonConverter<TradingPlanId>
{
    public override TradingPlanId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
            throw new JsonException("Expected a string for TradingPlanId.");

        var guidString = reader.GetString();
        if (!Guid.TryParse(guidString, out var guid))
            throw new JsonException("Invalid Guid format for TradingPlanId.");

        return TradingPlanId.Of(guid);
    }

    public override void Write(Utf8JsonWriter writer, TradingPlanId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value.ToString());
    }
}

public class TradeIdConverter : JsonConverter<TradeId>
{
    public override TradeId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
            throw new JsonException("Expected a string for TradeId.");

        var guidString = reader.GetString();
        if (!Guid.TryParse(guidString, out var guid))
            throw new JsonException("Invalid Guid format for TradeId.");

        return TradeId.Of(guid);
    }

    public override void Write(Utf8JsonWriter writer, TradeId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value.ToString());
    }
}
