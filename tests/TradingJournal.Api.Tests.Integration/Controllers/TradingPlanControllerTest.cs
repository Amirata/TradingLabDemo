using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using BuildingBlocks.Pagination;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using TradingJournal.Application.TradingPlans.Commands.CreateTradingPlan;
using TradingJournal.Application.TradingPlans.Commands.UpdateTradingPlan;
using TradingJournal.Application.TradingPlans.Queries.GetTradingPlanById;
using TradingJournal.Application.TradingPlans.Queries.GetTradingPlanByName;
using TradingJournal.Application.TradingPlans.Queries.GetTradingPlans;
using TradingJournal.Application.TradingTechnics.Commands.CreateTradingTechnic;
using TradingJournal.Application.TradingTechnics.Queries.GetTradingTechnicById;
using TradingJournal.Domain.Models;
using TradingJournal.Domain.ValueObjects;
using TradingJournal.Infrastructure.Data;
using Xunit.Abstractions;

namespace TradingJournal.Api.Tests.Integration.Controllers;

[Collection("Test collection")]
public class TradingPlanControllerTest : IClassFixture<TradingPlanClassFixture>
{
    private readonly HttpClient _client;
    private readonly TestLogger _logger;

    private readonly ApplicationDbContext _dbContext;
    private readonly TradingPlanClassFixture _fixture;
    private readonly ITestOutputHelper _testOutputHelper;

    public TradingPlanControllerTest(
        TradingJournalApiFactory apiFactory,
        ITestOutputHelper testOutputHelper,
        TradingPlanClassFixture fixture)
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
        
        _testOutputHelper = testOutputHelper;
        _logger = apiFactory.LoggerProvider.Logger;

        // Access the ApplicationDbContext through the DI container
        var scope = apiFactory.Services.CreateScope();
        _dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }

    #region Create

    [Fact, TestPriority(1)]
    public async Task Create_CreateTradingPlan_WhenDataIsValid()
    {
        // Arrange
        SeedData();

        var createTradingPlanCommand = new CreateTradingPlanCommand
        {
            Name = "name 1",
            FromTime = new TimeOnly(00, 00, 00),
            ToTime = new TimeOnly(01, 30, 00),
            Technics = _fixture.TradingTechnics.Select(t => t.Id).Take(1).ToList(),
            SelectedDays = new List<string>
            {
                DayOfWeek.Saturday.ToString(),
                DayOfWeek.Monday.ToString().ToLower()
            },
            UserId = Guid.Parse(_fixture.UserId)
        };


        _fixture.CreateTradingPlanCommand = createTradingPlanCommand;

        // Act
        var response =
            await _client.PostAsJsonAsync(_fixture.Uri, createTradingPlanCommand);

        // Assert
        var tradingPlanResponse =
            await response.Content.ReadFromJsonAsync<CreateTradingPlanResult>();
        _fixture.Id = tradingPlanResponse!.Id;
        tradingPlanResponse.Should().BeOfType<CreateTradingPlanResult>();
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location!.ToString().Should()
            .Be($"http://localhost/{_fixture.Uri}/{tradingPlanResponse.Id}");
    }

    [Fact, TestPriority(2)]
    public async Task Create_ReturnsValidationError_WhenNameIsNullOrEmpty()
    {
        // Arrange
        var createTradingPlanCommand = new CreateTradingPlanCommand
        {
            Name = "",
            FromTime = new TimeOnly(00, 00, 00),
            ToTime = new TimeOnly(01, 00, 00),
            Technics = _fixture.TradingTechnics.Select(t => t.Id).Take(1).ToList(),
            SelectedDays = [],
            UserId = Guid.Parse(_fixture.UserId)
        };

        // Act
        var response =
            await _client.PostAsJsonAsync(_fixture.Uri, createTradingPlanCommand);

        // Assert
        var problemDetailsResponse =
            await response.Content.ReadFromJsonAsync<ProblemDetails>();

        problemDetailsResponse.Should().BeOfType<ProblemDetails>();
        problemDetailsResponse!.Status.Should().Be(400);
        problemDetailsResponse.Title.Should().Be("ValidationException");
        problemDetailsResponse.Extensions.Keys.Should().Contain("ValidationErrors");
        problemDetailsResponse.Detail.Should().Contain("Name is required.");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        _logger.LogMessages.Should().ContainMatch("Error Message:*");

        ShowLogs();
    }

    [Fact, TestPriority(3)]
    public async Task Create_ReturnsValidationError_WhenFromTimeGreaterThanToTime()
    {
        // Arrange
        var createTradingPlanCommand = new CreateTradingPlanCommand
        {
            Name = "name 1",
            FromTime = new TimeOnly(23, 00, 00),
            ToTime = new TimeOnly(01, 00, 00),
            Technics = _fixture.TradingTechnics.Select(t => t.Id).Take(1).ToList(),
            SelectedDays = [],
            UserId = Guid.Parse(_fixture.UserId)
        };

        // Act
        var response =
            await _client.PostAsJsonAsync(_fixture.Uri, createTradingPlanCommand);

        // Assert
        var problemDetailsResponse =
            await response.Content.ReadFromJsonAsync<ProblemDetails>();

        problemDetailsResponse.Should().BeOfType<ProblemDetails>();
        problemDetailsResponse!.Status.Should().Be(400);
        problemDetailsResponse.Title.Should().Be("ValidationException");
        problemDetailsResponse.Extensions.Keys.Should().Contain("ValidationErrors");
        problemDetailsResponse.Detail.Should().Contain("ToTime must be greater than FromTime.");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        _logger.LogMessages.Should().ContainMatch("Error Message:*");

        ShowLogs();
    }

    [Fact, TestPriority(4)]
    public async Task Create_ReturnsValidationError_WhenFromTimeHasValueAndToTimeIsNull()
    {
        // Arrange
        var createTradingPlanCommand = new CreateTradingPlanCommand
        {
            Name = "name 1",
            FromTime = new TimeOnly(21, 00, 00),
            ToTime = null,
            Technics = _fixture.TradingTechnics.Select(t => t.Id).Take(1).ToList(),
            SelectedDays = [],
            UserId = Guid.Parse(_fixture.UserId)
        };

        // Act
        var response =
            await _client.PostAsJsonAsync(_fixture.Uri, createTradingPlanCommand);

        // Assert
        var problemDetailsResponse =
            await response.Content.ReadFromJsonAsync<ProblemDetails>();

        problemDetailsResponse.Should().BeOfType<ProblemDetails>();
        problemDetailsResponse!.Status.Should().Be(400);
        problemDetailsResponse.Title.Should().Be("ValidationException");
        problemDetailsResponse.Extensions.Keys.Should().Contain("ValidationErrors");
        problemDetailsResponse.Detail.Should().Contain("Both FromTime and ToTime must either be null or have values.");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        _logger.LogMessages.Should().ContainMatch("Error Message:*");

        ShowLogs();
    }

    [Fact, TestPriority(5)]
    public async Task Create_ReturnsValidationError_WhenFromTimeIsNullAndToTimeHasValue()
    {
        // Arrange
        var createTradingPlanCommand = new CreateTradingPlanCommand
        {
            Name = "name 1",
            FromTime = null,
            ToTime = new TimeOnly(21, 00, 00),
            Technics = _fixture.TradingTechnics.Select(t => t.Id).Take(1).ToList(),
            SelectedDays = [],
            UserId = Guid.Parse(_fixture.UserId)
        };

        // Act
        var response =
            await _client.PostAsJsonAsync(_fixture.Uri, createTradingPlanCommand);

        // Assert
        var problemDetailsResponse =
            await response.Content.ReadFromJsonAsync<ProblemDetails>();

        problemDetailsResponse.Should().BeOfType<ProblemDetails>();
        problemDetailsResponse!.Status.Should().Be(400);
        problemDetailsResponse.Title.Should().Be("ValidationException");
        problemDetailsResponse.Extensions.Keys.Should().Contain("ValidationErrors");
        problemDetailsResponse.Detail.Should().Contain("Both FromTime and ToTime must either be null or have values.");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        _logger.LogMessages.Should().ContainMatch("Error Message:*");

        ShowLogs();
    }

    [Fact, TestPriority(6)]
    public async Task Create_ReturnsValidationError_WhenTechnicsEmpty()
    {
        // Arrange
        var createTradingPlanCommand = new CreateTradingPlanCommand
        {
            Name = "name 1",
            FromTime = null,
            ToTime = null,
            Technics = [],
            SelectedDays = [],
            UserId = Guid.Parse(_fixture.UserId)
        };

        // Act
        var response =
            await _client.PostAsJsonAsync(_fixture.Uri, createTradingPlanCommand);

        // Assert
        var problemDetailsResponse =
            await response.Content.ReadFromJsonAsync<ProblemDetails>();

        problemDetailsResponse.Should().BeOfType<ProblemDetails>();
        problemDetailsResponse!.Status.Should().Be(400);
        problemDetailsResponse.Title.Should().Be("ValidationException");
        problemDetailsResponse.Extensions.Keys.Should().Contain("ValidationErrors");
        problemDetailsResponse.Detail.Should().Contain("Technics is required.");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        _logger.LogMessages.Should().ContainMatch("Error Message:*");

        ShowLogs();
    }

    [Fact, TestPriority(7)]
    public async Task Create_ReturnsDomainException_WhenTechnicsHasEmptyGuid()
    {
        // Arrange
        var createTradingPlanCommand = new CreateTradingPlanCommand
        {
            Name = "name 1",
            FromTime = null,
            ToTime = null,
            Technics = [new Guid()],
            SelectedDays = [],
            UserId = Guid.Parse(_fixture.UserId)
        };

        // Act
        var response =
            await _client.PostAsJsonAsync(_fixture.Uri, createTradingPlanCommand);

        // Assert
        var problemDetailsResponse =
            await response.Content.ReadFromJsonAsync<ProblemDetails>();

        problemDetailsResponse.Should().BeOfType<ProblemDetails>();
        problemDetailsResponse!.Status.Should().Be(500);
        problemDetailsResponse.Title.Should().Be("DomainException");
        problemDetailsResponse.Detail.Should().Contain("TradingTechnicId cannot be empty.");
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        _logger.LogMessages.Should().ContainMatch("Error Message:*");

        ShowLogs();
    }

    [Fact, TestPriority(8)]
    public async Task Create_ReturnsValidationError_WhenSelectedDaysHasInvalidValues()
    {
        // Arrange
        var createTradingPlanCommand = new CreateTradingPlanCommand
        {
            Name = "name 1",
            FromTime = null,
            ToTime = null,
            Technics = _fixture.TradingTechnics.Select(t => t.Id).Take(1).ToList(),
            SelectedDays = [DayOfWeek.Friday.ToString(), "Invalid"],
            UserId = Guid.Parse(_fixture.UserId)
        };

        // Act
        var response =
            await _client.PostAsJsonAsync(_fixture.Uri, createTradingPlanCommand);

        // Assert
        var problemDetailsResponse =
            await response.Content.ReadFromJsonAsync<ProblemDetails>();

        problemDetailsResponse.Should().BeOfType<ProblemDetails>();
        problemDetailsResponse!.Status.Should().Be(400);
        problemDetailsResponse.Title.Should().Be("ValidationException");
        problemDetailsResponse.Extensions.Keys.Should().Contain("ValidationErrors");
        problemDetailsResponse.Detail.Should().Contain("Selected days is not valid.");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        _logger.LogMessages.Should().ContainMatch("Error Message:*");

        ShowLogs();
    }

    #endregion

    #region GetById

    [Fact, TestPriority(9)]
    public async Task GetById_ReturnGetTradingPlanByIdResult_WhenTradingPlanExists()
    {
        // Arrange
        var getTradingPlanByIdResult = new GetTradingPlanByIdResult
        {
            Id = _fixture.Id,
            Name = _fixture.CreateTradingPlanCommand!.Name,
            FromTime = _fixture.CreateTradingPlanCommand!.FromTime,
            ToTime = _fixture.CreateTradingPlanCommand!.ToTime,
            Technics = _fixture.TradingTechnics.Take(1).ToList(),
            SelectedDays = _fixture.CreateTradingPlanCommand!.SelectedDays.Select(x => char.ToUpper(x[0]) + x[1..])
                .ToList(),
        };

        // Act
        var response = await _client.GetAsync($"{_fixture.Uri}/{_fixture.Id}");

        // Assert
        var retrievedTradingPlan = await response.Content.ReadFromJsonAsync<GetTradingPlanByIdResult>();
        retrievedTradingPlan!.Should().BeEquivalentTo(getTradingPlanByIdResult);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact, TestPriority(10)]
    public async Task GetById_ReturnsNotFound_WhenTradingPlanDoesNotExist()
    {
        // Act
        var response = await _client.GetAsync($"{_fixture.Uri}/{Guid.NewGuid()}");

        // Assert
        var problemDetailsResponse =
            await response.Content.ReadFromJsonAsync<ProblemDetails>();

        problemDetailsResponse.Should().BeOfType<ProblemDetails>();
        problemDetailsResponse!.Status.Should().Be(404);
    }

    #endregion

    #region Update

    [Fact, TestPriority(11)]
    public async Task Update_UpdatesTradingPlan_WhenDataIsValid()
    {
        // Arrange
        var updateTradingPlanCommand = new UpdateTradingPlanCommand
        {
            Id = _fixture.Id,
            Name = "updated name 1",
            FromTime = null,
            ToTime = null,
            Technics = [],
            RemovedTechnics = [],
            SelectedDays = []
        };

        _fixture.UpdateTradingPlanCommand = updateTradingPlanCommand;

        // Act
        var response =
            await _client.PutAsJsonAsync($"{_fixture.Uri}/{updateTradingPlanCommand.Id}", updateTradingPlanCommand);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact, TestPriority(12)]
    public async Task Update_ReturnsValidationError_WhenIdIsNullOrEmpty()
    {
        // Arrange
        var updateTradingPlanCommand = new UpdateTradingPlanCommand
        {
            Id = Guid.Empty,
            Name = "name 1",
            FromTime = new TimeOnly(00, 00, 00),
            ToTime = new TimeOnly(01, 00, 00),
            Technics = _fixture.TradingTechnics.Select(t => t.Id).Take(1).ToList(),
            RemovedTechnics = [],
            SelectedDays = []
        };

        // Act
        var response =
            await _client.PutAsJsonAsync($"{_fixture.Uri}/{updateTradingPlanCommand.Id}", updateTradingPlanCommand);

        // Assert
        var problemDetailsResponse =
            await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetailsResponse.Should().BeOfType<ProblemDetails>();
        problemDetailsResponse!.Status.Should().Be(400);
        problemDetailsResponse.Title.Should().Be("ValidationException");
        problemDetailsResponse.Extensions.Keys.Should().Contain("ValidationErrors");
        problemDetailsResponse.Detail.Should().Contain("Id is required.");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        _logger.LogMessages.Should().ContainMatch("Error Message:*");

        ShowLogs();
    }

    [Fact, TestPriority(13)]
    public async Task Update_ReturnsValidationError_WhenNameIsNullOrEmpty()
    {
        // Arrange
        var updateTradingPlanCommand = new UpdateTradingPlanCommand
        {
            Id = _fixture.Id,
            Name = "",
            FromTime = new TimeOnly(00, 00, 00),
            ToTime = new TimeOnly(01, 00, 00),
            Technics = _fixture.TradingTechnics.Select(t => t.Id).Take(1).ToList(),
            RemovedTechnics = [],
            SelectedDays = []
        };

        // Act
        var response =
            await _client.PutAsJsonAsync($"{_fixture.Uri}/{updateTradingPlanCommand.Id}", updateTradingPlanCommand);

        // Assert
        var problemDetailsResponse =
            await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetailsResponse.Should().BeOfType<ProblemDetails>();
        problemDetailsResponse!.Status.Should().Be(400);
        problemDetailsResponse.Title.Should().Be("ValidationException");
        problemDetailsResponse.Extensions.Keys.Should().Contain("ValidationErrors");
        problemDetailsResponse.Detail.Should().Contain("Name is required.");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        _logger.LogMessages.Should().ContainMatch("Error Message:*");

        ShowLogs();
    }

    [Fact, TestPriority(14)]
    public async Task Update_ReturnsValidationError_WhenFromTimeGreaterThanToTime()
    {
        // Arrange
        var updateTradingPlanCommand = new UpdateTradingPlanCommand
        {
            Id = _fixture.Id,
            Name = "name 1",
            FromTime = new TimeOnly(23, 59, 59),
            ToTime = new TimeOnly(21, 59, 59),
            Technics = _fixture.TradingTechnics.Select(t => t.Id).Take(1).ToList(),
            RemovedTechnics = [],
            SelectedDays = []
        };

        // Act
        var response =
            await _client.PutAsJsonAsync($"{_fixture.Uri}/{updateTradingPlanCommand.Id}", updateTradingPlanCommand);

        // Assert
        var problemDetailsResponse =
            await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetailsResponse.Should().BeOfType<ProblemDetails>();
        problemDetailsResponse!.Status.Should().Be(400);
        problemDetailsResponse.Title.Should().Be("ValidationException");
        problemDetailsResponse.Extensions.Keys.Should().Contain("ValidationErrors");
        problemDetailsResponse.Detail.Should().Contain("ToTime must be greater than FromTime.");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        _logger.LogMessages.Should().ContainMatch("Error Message:*");

        ShowLogs();
    }

    [Fact, TestPriority(15)]
    public async Task Update_ReturnsValidationError_WhenFromTimeHasValueAndToTimeIsNull()
    {
        // Arrange
        var updateTradingPlanCommand = new UpdateTradingPlanCommand
        {
            Id = _fixture.Id,
            Name = "name 1",
            FromTime = new TimeOnly(23, 59, 59),
            ToTime = null,
            Technics = _fixture.TradingTechnics.Select(t => t.Id).Take(1).ToList(),
            RemovedTechnics = [],
            SelectedDays = []
        };

        // Act
        var response =
            await _client.PutAsJsonAsync($"{_fixture.Uri}/{updateTradingPlanCommand.Id}", updateTradingPlanCommand);

        // Assert
        var problemDetailsResponse =
            await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetailsResponse.Should().BeOfType<ProblemDetails>();
        problemDetailsResponse!.Status.Should().Be(400);
        problemDetailsResponse.Title.Should().Be("ValidationException");
        problemDetailsResponse.Extensions.Keys.Should().Contain("ValidationErrors");
        problemDetailsResponse.Detail.Should().Contain("Both FromTime and ToTime must either be null or have values.");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        _logger.LogMessages.Should().ContainMatch("Error Message:*");

        ShowLogs();
    }

    [Fact, TestPriority(16)]
    public async Task Update_ReturnsValidationError_WhenFromTimeIsNullAndToTimeHasValue()
    {
        // Arrange
        var updateTradingPlanCommand = new UpdateTradingPlanCommand
        {
            Id = _fixture.Id,
            Name = "name 1",
            FromTime = null,
            ToTime = new TimeOnly(23, 59, 59),
            Technics = _fixture.TradingTechnics.Select(t => t.Id).Take(1).ToList(),
            RemovedTechnics = [],
            SelectedDays = []
        };

        // Act
        var response =
            await _client.PutAsJsonAsync($"{_fixture.Uri}/{updateTradingPlanCommand.Id}", updateTradingPlanCommand);

        // Assert
        var problemDetailsResponse =
            await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetailsResponse.Should().BeOfType<ProblemDetails>();
        problemDetailsResponse!.Status.Should().Be(400);
        problemDetailsResponse.Title.Should().Be("ValidationException");
        problemDetailsResponse.Extensions.Keys.Should().Contain("ValidationErrors");
        problemDetailsResponse.Detail.Should().Contain("Both FromTime and ToTime must either be null or have values.");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        _logger.LogMessages.Should().ContainMatch("Error Message:*");

        ShowLogs();
    }

    [Fact, TestPriority(17)]
    public async Task Update_ReturnsDomainException_WhenTechnicsHasEmptyGuid()
    {
        // Arrange
        var updateTradingPlanCommand = new UpdateTradingPlanCommand
        {
            Id = _fixture.Id,
            Name = "name 1",
            FromTime = new TimeOnly(00, 59, 59),
            ToTime = new TimeOnly(23, 59, 59),
            Technics = [new Guid()],
            RemovedTechnics = [],
            SelectedDays = []
        };

        // Act
        var response =
            await _client.PutAsJsonAsync($"{_fixture.Uri}/{updateTradingPlanCommand.Id}", updateTradingPlanCommand);

        // Assert
        var problemDetailsResponse =
            await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetailsResponse.Should().BeOfType<ProblemDetails>();
        problemDetailsResponse!.Status.Should().Be(500);
        problemDetailsResponse.Title.Should().Be("DomainException");
        problemDetailsResponse.Detail.Should().Contain("TradingTechnicId cannot be empty.");
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        _logger.LogMessages.Should().ContainMatch("Error Message:*");

        ShowLogs();
    }

    [Fact, TestPriority(18)]
    public async Task Update_ReturnsDomainException_WhenRemovedTechnicsHasEmptyGuid()
    {
        // Arrange
        var updateTradingPlanCommand = new UpdateTradingPlanCommand
        {
            Id = _fixture.Id,
            Name = "name 1",
            FromTime = new TimeOnly(21, 59, 59),
            ToTime = new TimeOnly(23, 59, 59),
            Technics = _fixture.TradingTechnics.Select(t => t.Id).Take(1).ToList(),
            RemovedTechnics = [new Guid()],
            SelectedDays = []
        };

        // Act
        var response =
            await _client.PutAsJsonAsync($"{_fixture.Uri}/{updateTradingPlanCommand.Id}", updateTradingPlanCommand);

        // Assert
        var problemDetailsResponse =
            await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetailsResponse.Should().BeOfType<ProblemDetails>();
        problemDetailsResponse!.Status.Should().Be(500);
        problemDetailsResponse.Title.Should().Be("DomainException");
        problemDetailsResponse.Detail.Should().Contain("TradingTechnicId cannot be empty.");
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        _logger.LogMessages.Should().ContainMatch("Error Message:*");

        ShowLogs();
    }

    [Fact, TestPriority(19)]
    public async Task Update_ReturnsValidationError_WhenTechnicsHasRemovedTechnicsIds()
    {
        // Arrange
        var updateTradingPlanCommand = new UpdateTradingPlanCommand
        {
            Id = _fixture.Id,
            Name = "name 1",
            FromTime = new TimeOnly(21, 59, 59),
            ToTime = new TimeOnly(23, 59, 59),
            Technics = _fixture.TradingTechnics.Select(t => t.Id).Take(2).ToList(),
            RemovedTechnics = _fixture.TradingTechnics.Select(t => t.Id).Take(1).ToList(),
            SelectedDays = []
        };

        // Act
        var response =
            await _client.PutAsJsonAsync($"{_fixture.Uri}/{updateTradingPlanCommand.Id}", updateTradingPlanCommand);

        // Assert
        var problemDetailsResponse =
            await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetailsResponse.Should().BeOfType<ProblemDetails>();
        problemDetailsResponse!.Status.Should().Be(400);
        problemDetailsResponse.Title.Should().Be("ValidationException");
        problemDetailsResponse.Extensions.Keys.Should().Contain("ValidationErrors");
        problemDetailsResponse.Detail.Should()
            .Contain("The Technics contains values that are also present in the RemovedTechnics.");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        _logger.LogMessages.Should().ContainMatch("Error Message:*");

        ShowLogs();
    }

    [Fact, TestPriority(20)]
    public async Task Update_ReturnsValidationError_WhenSelectedDaysHasInvalidValues()
    {
        // Arrange
        var updateTradingPlanCommand = new UpdateTradingPlanCommand
        {
            Id = _fixture.Id,
            Name = "name 1",
            FromTime = new TimeOnly(00, 00, 00),
            ToTime = new TimeOnly(01, 00, 00),
            Technics = _fixture.TradingTechnics.Select(t => t.Id).Take(1).ToList(),
            RemovedTechnics = [],
            SelectedDays = ["Invalid"]
        };

        // Act
        var response =
            await _client.PutAsJsonAsync($"{_fixture.Uri}/{updateTradingPlanCommand.Id}", updateTradingPlanCommand);

        // Assert
        var problemDetailsResponse =
            await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetailsResponse.Should().BeOfType<ProblemDetails>();
        problemDetailsResponse!.Status.Should().Be(400);
        problemDetailsResponse.Title.Should().Be("ValidationException");
        problemDetailsResponse.Extensions.Keys.Should().Contain("ValidationErrors");
        problemDetailsResponse.Detail.Should().Contain("Selected days is not valid.");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        _logger.LogMessages.Should().ContainMatch("Error Message:*");

        ShowLogs();
    }

    [Fact, TestPriority(21)]
    public async Task Update_ReturnsBadRequest_WhenIdIsInvalid()
    {
        // Arrange

        // Act
        var response =
            await _client.PutAsJsonAsync($"{_fixture.Uri}/{Guid.NewGuid()}", _fixture.UpdateTradingPlanCommand);

        // Assert
        var problemDetailsResponse =
            await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetailsResponse.Should().BeOfType<ProblemDetails>();
        problemDetailsResponse!.Status.Should().Be(400);
        problemDetailsResponse.Title.Should().Be("BadRequestException");
        problemDetailsResponse.Detail.Should().Be("The request Id does not match");

        ShowLogs();
    }

    #endregion

    #region GetByName

    [Fact, TestPriority(22)]
    public async Task GetByName_ReturnGetTradingPlanByNameResultList_WhenTradingPlanExists()
    {
        // Arrange
        var getTradingPlanByNameResult = new GetTradingPlanByNameResult
        {
            Id = _fixture.Id,
            Name = _fixture.UpdateTradingPlanCommand!.Name,
            FromTime = _fixture.UpdateTradingPlanCommand!.FromTime,
            ToTime = _fixture.UpdateTradingPlanCommand!.ToTime,
            Technics = _fixture.TradingTechnics.Take(1).ToList(),
            SelectedDays = _fixture.UpdateTradingPlanCommand!.SelectedDays.ToList(),
        };

        // Act
        var response = await _client.GetAsync($"{_fixture.Uri}/{_fixture.UpdateTradingPlanCommand!.Name}");

        // Assert
        var retrievedTradingPlan = await response.Content.ReadFromJsonAsync<ICollection<GetTradingPlanByNameResult>>();
        retrievedTradingPlan!.Should().BeEquivalentTo([getTradingPlanByNameResult]);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact, TestPriority(23)]
    public async Task GetByName_ReturnsEmptyList_WhenTradingPlanDoesNotExist()
    {
        // Act
        var response = await _client.GetAsync($"{_fixture.Uri}/Abc");

        // Assert

        var retrievedTradingPlan = await response.Content.ReadFromJsonAsync<ICollection<GetTradingPlanByNameResult>>();
        retrievedTradingPlan!.Should().BeEquivalentTo(new List<GetTradingPlanByNameResult>());
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion

    #region Delete

    [Fact, TestPriority(24)]
    public async Task Delete_DeleteTradingPlan_WhenTradingPlanExists()
    {
        // Arrange

        // Act
        var response = await _client.DeleteAsync($"{_fixture.Uri}/{_fixture.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact, TestPriority(25)]
    public async Task Delete_ReturnsNotFound_WhenTradingPlanDoesNotExist()
    {
        // Act
        var response = await _client.DeleteAsync($"{_fixture.Uri}/{Guid.NewGuid()}");

        // Assert
        var problemDetailsResponse =
            await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetailsResponse.Should().BeOfType<ProblemDetails>();
        problemDetailsResponse!.Status.Should().Be(404);
        problemDetailsResponse.Title.Should().Be("NotFoundException");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        ShowLogs();
    }

    #endregion

    #region GetAll

    [Fact, TestPriority(26)]
    public async Task GetAll_ReturnsEmptyResult_WhenNoTradingPlansExistWithDefaultQuery()
    {
        // Arrange
        var paginatedResult = new PaginatedResult<GetTradingPlansResult>([],0, 1, 5);

        // Act
        var response = await _client.GetAsync($"{_fixture.Uri}?pageSize=5&pageNumber=1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var tradingPlansResponse = await response.Content.ReadFromJsonAsync<PaginatedResult<GetTradingPlansResult>>();
        tradingPlansResponse.Should().BeOfType<PaginatedResult<GetTradingPlansResult>>();
        tradingPlansResponse!.Should().BeEquivalentTo(paginatedResult);
    }

    [Fact, TestPriority(27)]
    public async Task GetAll_ReturnsFirstFiveTradingPlansOfNineteen_WhenPageNumberOneAndPageSizeFive()
    {
        // Arrange
        var getTradingPlansResult = new List<GetTradingPlansResult>();
        var random = new Random();
        var days = new List<string>
        {
            DayOfWeek.Monday.ToString(),
            DayOfWeek.Tuesday.ToString(),
            DayOfWeek.Wednesday.ToString(),
            DayOfWeek.Thursday.ToString(),
            DayOfWeek.Friday.ToString(),
            DayOfWeek.Saturday.ToString(),
            DayOfWeek.Sunday.ToString()
        };

        for (var i = 1; i <= 19; i++)
        {
            var deyIndex = random.Next(days.Count);
            var createTradingPlanCommand = new CreateTradingPlanCommand
            {
                Name = $"name {i}",
                FromTime = new TimeOnly(i, 00, 00),
                ToTime = new TimeOnly(23, i, 00),
                Technics = _fixture.TradingTechnics.Select(t => t.Id).Take(random.Next(1, 4)).ToList(),
                SelectedDays = new List<string>
                {
                    days[deyIndex]
                }, 
                UserId = Guid.Parse(_fixture.UserId)
                
            };

            var createResponse = await _client.PostAsJsonAsync(_fixture.Uri, createTradingPlanCommand);
            var createResult = await createResponse.Content.ReadFromJsonAsync<CreateTradingPlanResult>();

            getTradingPlansResult.Add(new GetTradingPlansResult
            {
                Id = createResult!.Id,
                Name = createTradingPlanCommand.Name,
                FromTime = createTradingPlanCommand.FromTime,
                ToTime = createTradingPlanCommand.ToTime,
                SelectedDays = createTradingPlanCommand.SelectedDays.ToList(),
                Technics = _fixture.TradingTechnics.Where(t => createTradingPlanCommand.Technics.Contains(t.Id))
                    .ToList()
                
            });
        }

        _fixture.GetTradingPlansResultList = getTradingPlansResult.ToList();

        var paginatedResult =
            new PaginatedResult<GetTradingPlansResult>(getTradingPlansResult.Skip(0).Take(5).ToList(),19, 1, 5 );


        // Act
        var response = await _client.GetAsync($"{_fixture.Uri}?pageSize=5&pageNumber=1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var tradingPlansResponse = await response.Content.ReadFromJsonAsync<PaginatedResult<GetTradingPlansResult>>();
        tradingPlansResponse.Should().BeOfType<PaginatedResult<GetTradingPlansResult>>();
        tradingPlansResponse!.Data.Should().HaveCount(5);
        tradingPlansResponse.Should().BeEquivalentTo(paginatedResult);
    }

    [Fact, TestPriority(28)]
    public async Task GetAll_ReturnsLastFourTradingPlansOfNineteen_WhenPageNumberFiveAndPageSizeFive()
    {
        // Arrange
        var paginatedResult =
            new PaginatedResult<GetTradingPlansResult>(_fixture.GetTradingPlansResultList.Skip(15).Take(5).ToList(),19, 4, 5);

        // Act
        var response = await _client.GetAsync($"{_fixture.Uri}?pageSize=5&pageNumber=4");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var tradingPlansResponse = await response.Content.ReadFromJsonAsync<PaginatedResult<GetTradingPlansResult>>();
        tradingPlansResponse.Should().BeOfType<PaginatedResult<GetTradingPlansResult>>();
        tradingPlansResponse!.Data.Should().HaveCount(4);
        tradingPlansResponse.Should().BeEquivalentTo(paginatedResult);
    }

    [Fact, TestPriority(29)]
    public async Task GetAll_ReturnsFirstFiveTradingPlans_WhenTradingPlansExistWithSearchQuery()
    {
        // Arrange
        var paginatedResult = new PaginatedResult<GetTradingPlansResult>(
            _fixture.GetTradingPlansResultList.Where(x => x.Name.Contains("e 1")).Skip(0).Take(5).ToList(),11, 1, 5);

        // Act
        var response = await _client.GetAsync($"{_fixture.Uri}?pageSize=5&pageNumber=1&search=e 1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var tradingPlansResponse = await response.Content.ReadFromJsonAsync<PaginatedResult<GetTradingPlansResult>>();
        tradingPlansResponse.Should().BeOfType<PaginatedResult<GetTradingPlansResult>>();
        tradingPlansResponse!.Data.Should().HaveCount(5);
        tradingPlansResponse.Should().BeEquivalentTo(paginatedResult);
    }

    [Fact, TestPriority(30)]
    public async Task GetAll_ReturnsEmptyResult_WhenTradingPlansNotExistWithSearchQuery()
    {
        // Arrange
        var paginatedResult = new PaginatedResult<GetTradingPlansResult>([],0, 1, 5 );

        // Act
        var response = await _client.GetAsync($"{_fixture.Uri}?pageSize=5&pageNumber=1&search=abcd");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var tradingPlansResponse = await response.Content.ReadFromJsonAsync<PaginatedResult<GetTradingPlansResult>>();
        tradingPlansResponse.Should().BeOfType<PaginatedResult<GetTradingPlansResult>>();
        tradingPlansResponse!.Data.Should().HaveCount(0);
        tradingPlansResponse.Should().BeEquivalentTo(paginatedResult);
    }

    [Fact, TestPriority(31)]
    public async Task GetAll_ReturnsFirstFiveAscOrderedTradingPlanOfNineteen_WhenOrderAscQueryValid()
    {
        // Arrange
        var paginatedResult = new PaginatedResult<GetTradingPlansResult>(
            _fixture.GetTradingPlansResultList.OrderBy(x => x.Name).Skip(0).Take(5).ToList()
            ,19, 1, 5);

        // Act
        //var response = await _client.GetAsync($"{_fixture.Uri}?pageSize=5&pageNumber=1&sortBy=Name&sortOrder=asc");
        var response = await _client.GetAsync($"{_fixture.Uri}?pageSize=5&pageNumber=1&search=&sorts[0].sortBy=Name&sorts[0].sortOrder=asc&sorts[0].order=1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var tradingPlansResponse = await response.Content.ReadFromJsonAsync<PaginatedResult<GetTradingPlansResult>>();
        tradingPlansResponse.Should().BeOfType<PaginatedResult<GetTradingPlansResult>>();
        tradingPlansResponse!.Data.Should().HaveCount(5);
        tradingPlansResponse.Should().BeEquivalentTo(paginatedResult);
    }

    [Fact, TestPriority(32)]
    public async Task GetAll_ReturnsFirstFiveDescOrderedTradingPlanOfNineteen_WhenOrderDescQueryValid()
    {
        // Arrange
        var paginatedResult = new PaginatedResult<GetTradingPlansResult>(
            _fixture.GetTradingPlansResultList.OrderByDescending(x => x.Name).Skip(0).Take(5).ToList(),19, 1, 5);

        // Act
        //var response = await _client.GetAsync($"{_fixture.Uri}?pageSize=5&pageNumber=1&sortBy=Name&sortOrder=desc");
        var response = await _client.GetAsync($"{_fixture.Uri}?pageSize=5&pageNumber=1&search=&sorts[0].sortBy=Name&sorts[0].sortOrder=desc&sorts[0].order=1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var tradingPlansResponse = await response.Content.ReadFromJsonAsync<PaginatedResult<GetTradingPlansResult>>();
        tradingPlansResponse.Should().BeOfType<PaginatedResult<GetTradingPlansResult>>();
        tradingPlansResponse!.Data.Should().HaveCount(5);
        tradingPlansResponse.Should().BeEquivalentTo(paginatedResult);
    }
    
    #endregion
    
    private void ShowLogs()
    {
        foreach (var message in _logger.LogMessages)
        {
            _testOutputHelper.WriteLine(message);
        }

        _logger.Clear();
    }
    
    private void SeedData()
    {
        
        _dbContext.Users.Add(User.Create(
            UserId.Of(Guid.Parse(_fixture.UserId)),
            _fixture.UserName
        ));
        
        var tradingTechnics1 = TradingTechnic.Create(
            TradingTechnicId.New(),
            "name 1",
            "description 1",
            UserId.Of(Guid.Parse(_fixture.UserId))
        );
        tradingTechnics1.AddImage("image 1");
        tradingTechnics1.AddImage("image 2");

        var tradingTechnics2 = TradingTechnic.Create(
            TradingTechnicId.New(),
            "name 2",
            "description 2",
            UserId.Of(Guid.Parse(_fixture.UserId))
        );
        tradingTechnics2.AddImage("image 3");
        tradingTechnics2.AddImage("image 4");

        var tradingTechnics3 = TradingTechnic.Create(
            TradingTechnicId.New(),
            "name 3",
            "description 3",
            UserId.Of(Guid.Parse(_fixture.UserId))
        );
        tradingTechnics3.AddImage("image 4");
        tradingTechnics3.AddImage("image 5");

        _dbContext.TradingTechnics.AddRange([
            tradingTechnics1,
            tradingTechnics2,
            tradingTechnics3
        ]);
        _fixture.TradingTechnics.AddRange(new List<GetTradingTechnicByIdResult>
        {
            new()
            {
                Id = tradingTechnics1.Id.Value,
                Name = tradingTechnics1.Name,
                Description = tradingTechnics1.Description,
                Images = tradingTechnics1.Images.Select(i => i.Path)
            },
            new()
            {
                Id = tradingTechnics2.Id.Value,
                Name = tradingTechnics2.Name,
                Description = tradingTechnics2.Description,
                Images = tradingTechnics2.Images.Select(i => i.Path)
            },
            new()
            {
                Id = tradingTechnics3.Id.Value,
                Name = tradingTechnics3.Name,
                Description = tradingTechnics3.Description,
                Images = tradingTechnics3.Images.Select(i => i.Path)
            }
        });

        _dbContext.SaveChanges();
    }
}