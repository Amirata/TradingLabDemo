using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TradingJournal.Application.TradeAnalyses.Queries.GetCalendarByYear;
using TradingJournal.Application.TradeAnalyses.Queries.GetChartNetProfit;
using TradingJournal.Application.TradeAnalyses.Queries.GetGrossAndNetForEachSymbol;
using TradingJournal.Application.TradeAnalyses.Queries.GetGrossAndNetForEachSymbolForEachDayOfWeek;
using TradingJournal.Domain.Enums;
using TradingJournal.Domain.Models;
using TradingJournal.Domain.ValueObjects;
using TradingJournal.Infrastructure.Data;


namespace TradingJournal.Api.Tests.Integration.Controllers;

[Collection("Test collection")]
public class TradeAnalyseControllerTest : IClassFixture<TradeAnalyseClassFixture>
{
    private readonly HttpClient _client;
    private readonly ApplicationDbContext _dbContext;
    private readonly TradeAnalyseClassFixture _fixture;
    
    public TradeAnalyseControllerTest(
        TradingJournalApiFactory apiFactory,
        TradeAnalyseClassFixture fixture)

    {
        _client = apiFactory.CreateClient();
        _fixture = fixture;
        
        var token = JwtHelper.GenerateJwtToken(
            _fixture.UserId,
            _fixture.UserName,
            secretKey: "SUPER_SECURE_KEY_HERE_1234567890", 
            issuer: "https://yourapp.com", 
            audience: "https://yourapp.com"
        );
        
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Access the ApplicationDbContext through the DI container
        var scope = apiFactory.Services.CreateScope();
        _dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        SeedData();
    }

    #region GetTradeYears

    [Fact]
    public async Task GetTradeYears_ReturnEmptyList_WhenPlanIdDoesNotExist()
    {
        // Arrange

        // Act
        var response = await _client.GetAsync($"{_fixture.Uri}/{Guid.NewGuid()}");

        // Assert
        var retrieved = await response.Content.ReadFromJsonAsync<IEnumerable<int>>();
        retrieved!.Should().HaveCount(0);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetTradeYears_ReturnEmptyList_WhenNoDataExist()
    {
        // Arrange
        await _dbContext.Database.ExecuteSqlRawAsync(@"TRUNCATE TABLE ""Trades""");

        // Act
        var response = await _client.GetAsync($"{_fixture.Uri}/{_fixture.PlanId1}");

        // Assert
        var retrieved = await response.Content.ReadFromJsonAsync<IEnumerable<int>>();
        retrieved!.Should().HaveCount(0);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetTradeYears_ReturnTwoYear_WhenDataExist()
    {
        // Arrange

        // Act
        var response = await _client.GetAsync($"{_fixture.Uri}/{_fixture.PlanId1}");

        // Assert
        var retrieved = await response.Content.ReadFromJsonAsync<ICollection<int>>();
        retrieved!.Should().HaveCount(2);
        retrieved!.Should().Contain([2025, 2021]);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion

    #region GetCalendarByYear

    [Fact]
    public async Task GetCalendarByYear_ReturnEmptyData_WhenPlanIdDoesNotExist()
    {
        // Arrange
     

        // Act
        var response = await _client.GetAsync($"{_fixture.Uri}/{Guid.NewGuid()}/{2020}");

        // Assert
        var retrieved = await response.Content.ReadFromJsonAsync<GetCalendarByYearResult>();
        retrieved!.Calendar.Should().BeNull();
        retrieved.RiskToRewardMean.Should().Be(0);
        retrieved.WinRate.Should().Be(0);
        retrieved.TotalTradeCount.Should().Be(0);
        retrieved.TotalWinTradeCount.Should().Be(0);
        retrieved.TotalLossTradeCount.Should().Be(0);
        retrieved.NetProfit.Should().Be(0);
        retrieved.GrossProfit.Should().Be(0);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetCalendarByYear_ReturnEmptyData_WhenYearDoesNotExist()
    {
        // Arrange

        // Act
        var response = await _client.GetAsync($"{_fixture.Uri}/{_fixture.PlanId1}/{2001}");

        // Assert
        var retrieved = await response.Content.ReadFromJsonAsync<GetCalendarByYearResult>();
        retrieved!.Calendar.Should().BeNull();
        retrieved.RiskToRewardMean.Should().Be(0);
        retrieved.WinRate.Should().Be(0);
        retrieved.TotalTradeCount.Should().Be(0);
        retrieved.TotalWinTradeCount.Should().Be(0);
        retrieved.TotalLossTradeCount.Should().Be(0);
        retrieved.NetProfit.Should().Be(0);
        retrieved.GrossProfit.Should().Be(0);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetCalendarByYear_ReturnEmptyData_WhenNoDataExist()
    {
        // Arrange
        await _dbContext.Database.ExecuteSqlRawAsync(@"TRUNCATE TABLE ""Trades""");
        
        // Act
        var response = await _client.GetAsync($"{_fixture.Uri}/{_fixture.PlanId1}/{2020}");

        // Assert
        var retrieved = await response.Content.ReadFromJsonAsync<GetCalendarByYearResult>();
        retrieved!.Calendar.Should().BeNull();
        retrieved.RiskToRewardMean.Should().Be(0);
        retrieved.WinRate.Should().Be(0);
        retrieved.TotalTradeCount.Should().Be(0);
        retrieved.TotalWinTradeCount.Should().Be(0);
        retrieved.TotalLossTradeCount.Should().Be(0);
        retrieved.NetProfit.Should().Be(0);
        retrieved.GrossProfit.Should().Be(0);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetCalendarByYear_ReturnData_WhenDataExist()
    {
        // Arrange
        
        // Act
        var response = await _client.GetAsync($"{_fixture.Uri}/{_fixture.PlanId1}/{2025}");

        // Assert
        var retrieved = await response.Content.ReadFromJsonAsync<GetCalendarByYearResult>();
        retrieved!.Calendar.Should().HaveCount(5);
        retrieved.Calendar.Should().BeEquivalentTo(new List<TradeCalendar>
            {
                new()
                {
                    Date = new DateOnly(2025, 01, 01),
                    Level = 1
                },
                new()
                {
                    Date = new DateOnly(2025, 01, 13),
                    Level = 0
                },
                new()
                {
                    Date = new DateOnly(2025, 02, 13),
                    Level = 2
                },
                new()
                {
                    Date = new DateOnly(2025, 03, 16),
                    Level = 0
                },
                new()
                {
                    Date = new DateOnly(2025, 12, 31),
                    Level = 1
                },
            }
        );
        retrieved.RiskToRewardMean.Should().Be(2.0);
        retrieved.WinRate.Should().Be(40.0);
        retrieved.TotalTradeCount.Should().Be(5);
        retrieved.TotalWinTradeCount.Should().Be(2);
        retrieved.TotalLossTradeCount.Should().Be(3);
        retrieved.NetProfit.Should().Be(-20.64);
        retrieved.GrossProfit.Should().Be(29.4);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion

    #region GetChartNetProfit

    [Fact]
    public async Task GetChartNetProfit_ReturnEmptyList_WhenPlanIdDoesNotExist()
    {
        // Arrange

        // Act
        var response = await _client.GetAsync($"{_fixture.Uri}/GetChartNetProfit/{Guid.NewGuid()}");

        // Assert
        var retrieved = await response.Content.ReadFromJsonAsync<ICollection<GetChartNetProfitResult>>();
        retrieved!.Should().HaveCount(0);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetChartNetProfit_ReturnEmptyList_WhenNoDataExist()
    {
        // Arrange
        await _dbContext.Database.ExecuteSqlRawAsync(@"TRUNCATE TABLE ""Trades""");
        
        // Act
        var response = await _client.GetAsync($"{_fixture.Uri}/GetChartNetProfit/{_fixture.PlanId1}");

        // Assert
        var retrieved = await response.Content.ReadFromJsonAsync<ICollection<GetChartNetProfitResult>>();
        retrieved!.Should().HaveCount(0);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetChartNetProfit_ReturnFourData_WhenDataExist()
    {
        // Arrange
        
        // Act
        var response = await _client.GetAsync($"{_fixture.Uri}/GetChartNetProfit/{_fixture.PlanId1}");

        // Assert
        var retrieved = await response.Content.ReadFromJsonAsync<ICollection<GetChartNetProfitResult>>();
        retrieved!.Should().HaveCount(4);
        retrieved!.Should().BeEquivalentTo(new List<GetChartNetProfitResult>
        {
            new()
            {
                Date = "2021-12-31",
                NetProfit = 150
            },
            new()
            {
                Date = "2025-01-13",
                NetProfit = -45.44
            },
            new()
            {
                Date = "2025-02-13",
                NetProfit = 70.24
            },
            new()
            {
                Date = "2025-03-16",
                NetProfit = -45.44
            },
        });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion
    
    #region GetGrossAndNetForEachSymbol

    [Fact]
    public async Task GetGrossAndNetForEachSymbol_ReturnEmptyList_WhenPlanIdDoesNotExist()
    {
        // Arrange

        // Act
        var response = await _client.GetAsync($"{_fixture.Uri}/GetGrossAndNetForEachSymbol/{Guid.NewGuid()}");

        // Assert
        var retrieved = await response.Content.ReadFromJsonAsync<ICollection<GetGrossAndNetForEachSymbolResult>>();
        retrieved!.Should().HaveCount(0);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task GetGrossAndNetForEachSymbol_ReturnEmptyList_WhenFromDateToDateDoesNotExist()
    {
        // Arrange

        // Act
        var response = await _client.GetAsync($"{_fixture.Uri}/GetGrossAndNetForEachSymbol/{_fixture.PlanId1}?fromDate=2026-01-05&toDate=2026-11-13");

        // Assert
        var retrieved = await response.Content.ReadFromJsonAsync<ICollection<GetGrossAndNetForEachSymbolResult>>();
        retrieved!.Should().HaveCount(0);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetGrossAndNetForEachSymbol_ReturnEmptyList_WhenNoDataExist()
    {
        // Arrange
        await _dbContext.Database.ExecuteSqlRawAsync(@"TRUNCATE TABLE ""Trades""");

        // Act
        var response = await _client.GetAsync($"{_fixture.Uri}/GetGrossAndNetForEachSymbol/{_fixture.PlanId1}");

        // Assert
        var retrieved = await response.Content.ReadFromJsonAsync<ICollection<GetGrossAndNetForEachSymbolResult>>();
        retrieved!.Should().HaveCount(0);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetGrossAndNetForEachSymbol_ReturnTwoData_WhenDataExist()
    {
        // Arrange

        // Act
        var response = await _client.GetAsync($"{_fixture.Uri}/GetGrossAndNetForEachSymbol/{_fixture.PlanId1}?fromDate=2025-01-01&toDate=2025-03-01");

        // Assert
        var retrieved = await response.Content.ReadFromJsonAsync<ICollection<GetGrossAndNetForEachSymbolResult>>();
        retrieved!.Should().HaveCount(2);
        retrieved!.Should().BeEquivalentTo(new List<GetGrossAndNetForEachSymbolResult>
        {
            new()
            {
               Symbol = Symbols.Us30,
               NetProfit = -121.81,
               GrossProfit = -95.1
            },
            new()
            {
                Symbol = Symbols.XauUsd,
                NetProfit = 146.61,
                GrossProfit = 160.0
            }
        });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    #endregion
    
    #region GetGrossAndNetForEachSymbolForEachDayOfWeek
    
     [Fact]
    public async Task GetGrossAndNetForEachSymbolForEachDayOfWeek_ReturnEmptyList_WhenPlanIdDoesNotExist()
    {
        // Arrange

        // Act
        var response = await _client.GetAsync($"{_fixture.Uri}/GetGrossAndNetForEachSymbolForEachDayOfWeek/{Guid.NewGuid()}");

        // Assert
        var retrieved = await response.Content.ReadFromJsonAsync<ICollection<GetGrossAndNetForEachSymbolForEachDayOfWeekResult>>();
        retrieved!.Should().HaveCount(0);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task GetGrossAndNetForEachSymbolForEachDayOfWeek_ReturnEmptyList_WhenFromDateToDateDoesNotExist()
    {
        // Arrange

        // Act
        var response = await _client.GetAsync($"{_fixture.Uri}/GetGrossAndNetForEachSymbolForEachDayOfWeek/{_fixture.PlanId1}?fromDate=2026-01-05&toDate=2026-11-13");

        // Assert
        var retrieved = await response.Content.ReadFromJsonAsync<ICollection<GetGrossAndNetForEachSymbolForEachDayOfWeekResult>>();
        retrieved!.Should().HaveCount(0);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetGrossAndNetForEachSymbolForEachDayOfWeek_ReturnEmptyList_WhenSymbolDoesNotExist()
    {
        // Arrange

        // Act
        var response = await _client.GetAsync($"{_fixture.Uri}/GetGrossAndNetForEachSymbolForEachDayOfWeek/{_fixture.PlanId1}?symbol=0");

        // Assert
        var retrieved = await response.Content.ReadFromJsonAsync<ICollection<GetGrossAndNetForEachSymbolForEachDayOfWeekResult>>();
        retrieved!.Should().HaveCount(0);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetGrossAndNetForEachSymbolForEachDayOfWeek_ReturnEmptyList_WhenNoDataExist()
    {
        // Arrange
        await _dbContext.Database.ExecuteSqlRawAsync(@"TRUNCATE TABLE ""Trades""");

        // Act
        var response = await _client.GetAsync($"{_fixture.Uri}/GetGrossAndNetForEachSymbolForEachDayOfWeek/{_fixture.PlanId1}");

        // Assert
        var retrieved = await response.Content.ReadFromJsonAsync<ICollection<GetGrossAndNetForEachSymbolForEachDayOfWeekResult>>();
        retrieved!.Should().HaveCount(0);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetGrossAndNetForEachSymbolForEachDayOfWeek_ReturnTwoData_WhenDataExist()
    {
        // Arrange

        // Act
        var response = await _client.GetAsync($"{_fixture.Uri}/GetGrossAndNetForEachSymbolForEachDayOfWeek/{_fixture.PlanId1}?fromDate=2025-01-01&toDate=2025-03-01&symbol=8");

        // Assert
        var retrieved = await response.Content.ReadFromJsonAsync<ICollection<GetGrossAndNetForEachSymbolForEachDayOfWeekResult>>();
        retrieved!.Should().HaveCount(2);
        retrieved!.Should().BeEquivalentTo(new List<GetGrossAndNetForEachSymbolForEachDayOfWeekResult>
        {
            new()
            {
               DayOfWeek = (int) DayOfWeek.Thursday, //2025-01-13
               NetProfit = -76.37,
               GrossProfit = -59.6
            },
            new()
            {
                DayOfWeek = (int) DayOfWeek.Monday , //2025-02-13
                NetProfit = -45.44,
                GrossProfit = -35.50
            },
          
        });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    #endregion
    
    private void SeedData()
    {
        _dbContext.Database.ExecuteSqlRaw(@"TRUNCATE TABLE ""TradingPlans"" CASCADE;");
        _dbContext.Database.ExecuteSqlRaw(@"TRUNCATE TABLE ""Users"" CASCADE;");

        _dbContext.Users.Add(User.Create(
             UserId.Of(Guid.Parse(_fixture.UserId)),
             _fixture.UserName
        ));

        var planId1 = TradingPlanId.New();
        _fixture.PlanId1 = planId1.Value;
        var tradingPlan1 = TradingPlan.Create(
            planId1,
            "Test plan name 1",
            null,
            null,
            [],
            UserId.Of(Guid.Parse(_fixture.UserId))
            
        );

        var technic1 = TradingTechnic.Create(
            TradingTechnicId.New(),
            "Test technic name 1",
            "Test technic description 1",
            UserId.Of(Guid.Parse(_fixture.UserId))
        );

        technic1.AddImage("pic1");

        tradingPlan1.AddTechnic(technic1);

        _dbContext.TradingPlans.Add(tradingPlan1);

        var planId2 = TradingPlanId.New();
        _fixture.PlanId2 = planId2.Value;
        var tradingPlan2 = TradingPlan.Create(
            planId2,
            "Test plan name 2",
            null,
            null,
            [],
            UserId.Of(Guid.Parse(_fixture.UserId))
        );

        var technic2 = TradingTechnic.Create(
            TradingTechnicId.New(),
            "Test technic name 2",
            "Test technic description 2",
            UserId.Of(Guid.Parse(_fixture.UserId))
        );

        technic2.AddImage("pic2");

        tradingPlan2.AddTechnic(technic2);

        _dbContext.TradingPlans.Add(tradingPlan2);
        
        var trade1 = Trade.Create(
            TradeId.New(),
            Symbols.Us30,
            PositionType.Short,
            1.0,
            41765.70,
            41752.10,
            41758.90,
            new DateTime(2025, 02, 13, 09, 49, 22),
            new DateTime(2025, 02, 13, 10, 06, 41),
            -8.36,
            0,
            13.6,
            5.24,
            13.60,
            996415.21,
            TradingPlanId.Of(_fixture.PlanId1)
        );
        var trade2 = Trade.Create(
            TradeId.New(),
            Symbols.XauUsd,
            PositionType.Short,
            1.0,
            2678.78,
            2677.18,
            2677.98,
            new DateTime(2025, 02, 13, 11, 34, 11),
            new DateTime(2025, 02, 13, 11, 37, 43),
            -13.39,
            0,
            160,
            146.61,
            160,
            996561.82,
            TradingPlanId.Of(_fixture.PlanId1)
        );
        var trade3 = Trade.Create(
            TradeId.New(),
            Symbols.Us30,
            PositionType.Short,
            1.0,
            42048.90,
            42122.10,
            42012.3,
            new DateTime(2025, 02, 13, 15, 10, 31),
            new DateTime(2025, 02, 13, 15, 18, 57),
            -8.41,
            0,
            -73.20,
            -81.61,
            -73.20,
            996480.21,
            TradingPlanId.Of(_fixture.PlanId1)
        );

        var trade4 = Trade.Create(
            TradeId.New(),
            Symbols.Us30,
            PositionType.Long,
            1.0,
            44716.30,
            44679.80,
            44698.05,
            new DateTime(2025, 01, 13, 15, 10, 31),
            new DateTime(2025, 01, 13, 15, 18, 57),
            -8.94,
            0,
            -36.50,
            -45.44,
            -35.50,
            996409.97,
            TradingPlanId.Of(_fixture.PlanId1)
        );
        
        var trade5 = Trade.Create(
            TradeId.New(),
            Symbols.Us30,
            PositionType.Long,
            1.0,
            44716.30,
            44679.80,
            44698.05,
            new DateTime(2025, 03, 16, 15, 10, 31),
            new DateTime(2025, 03, 16, 15, 18, 57),
            -8.94,
            0,
            -36.50,
            -45.44,
            -35.50,
            996409.97,
            TradingPlanId.Of(_fixture.PlanId1)
        );

        var trade6 = Trade.Create(
            TradeId.New(),
            Symbols.Us30,
            PositionType.Long,
            0.01,
            1233.24,
            1234.25,
            1132.25,
            new DateTime(2021, 12, 31, 20, 59, 59),
            new DateTime(2021, 12, 31, 23, 59, 59),
            0,
            0,
            200,
            150,
            160,
            1600,
            TradingPlanId.Of(_fixture.PlanId1)
        );
        var trade7 = Trade.Create(
            TradeId.New(),
            Symbols.Us30,
            PositionType.Long,
            0.01,
            1233.24,
            1234.25,
            1132.25,
            new DateTime(2020, 12, 31, 20, 59, 59),
            new DateTime(2020, 12, 31, 23, 59, 59),
            0,
            0,
            200,
            150,
            160,
            1600,
            TradingPlanId.Of(_fixture.PlanId2)
        );
        _dbContext.Trades.Add(trade1);
        _dbContext.Trades.Add(trade2);
        _dbContext.Trades.Add(trade3);
        _dbContext.Trades.Add(trade4);
        _dbContext.Trades.Add(trade5);
        _dbContext.Trades.Add(trade6);
        _dbContext.Trades.Add(trade7);
        
        
        _dbContext.SaveChanges();
    }
}