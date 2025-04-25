using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using BuildingBlocks.Pagination;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TradingJournal.Application.TradingTechnics.Commands.CreateTradingTechnic;
using TradingJournal.Application.TradingTechnics.Commands.UpdateTradingTechnic;
using TradingJournal.Application.TradingTechnics.Queries.GetTradingTechnicById;
using TradingJournal.Application.TradingTechnics.Queries.GetTradingTechnicByName;
using TradingJournal.Application.TradingTechnics.Queries.GetTradingTechnics;
using TradingJournal.Domain.Models;
using TradingJournal.Domain.ValueObjects;
using TradingJournal.Infrastructure.Data;
using Xunit.Abstractions;

namespace TradingJournal.Api.Tests.Integration.Controllers;

[Collection("Test collection")]
public class TradingTechnicControllerTest : IClassFixture<TradingTechnicClassFixture>
{
    private readonly HttpClient _client;
    private readonly TestLogger _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly TradingTechnicClassFixture _fixture;
    private readonly ITestOutputHelper _testOutputHelper;

    private readonly string _image1Base64 =
        "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAACQAAAAkCAYAAADhAJiYAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAAEnQAABJ0Ad5mH3gAAAEKSURBVFhH7da7ioUwFIXhpZjKRtResVJQfAHB92+91T6AilgbcvrNDFuSyByG/OVKig9R0Ou6TuGL8unw1zkQlwNxORCXA3E5ENf/AXmehzzP6WycNqgoCmRZhrqu6ZFR2qB1XXFdF5IkQdM09Fg7bZCUEsMw4LouxHFsDaUNwksoIxBeQHncL2wURXT6Md/3UZYlhBDY9x3TNNErj2JBfd/T6VHbtmGeZzqzsaC2ben0a2EYQggBKSWWZcFxHPQKGwt6WpqmqKoKADCOI87zpFceZfxSwyIGNkA2MTAF2cbABPQGBiYgpRSUUlYxMP3KgiDAfd90Nkr7CQGwjoEp6I0ciMuBuL4O9AELrnGMdSa3jQAAAABJRU5ErkJggg==";

    private readonly string _image2Base64 =
        "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAACEAAAAkCAYAAAAHKVPcAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAAEnQAABJ0Ad5mH3gAAAFUSURBVFhH7dYxisJAFIDhP5NpvISdxkILre3S6Ck05Az2ewtBsBeP4CU8gFgklVpYiaIibjWwPlaYSSI285VvXvETMiFBv99/8mVKDr7BRxg+wvARho8wfIRROkJrzWAwQGstj6yVitBaMx6PieOY4XAoj60VjlBKkSQJURSR5zmr1UquWCsUoZQiTVOazSZ5njObzbher3LNmnNE1QG4RnwiACCw/bMy70Cr1eJ0OrFcLrndbnLtX9vtVo5eWEeMRiPa7bYcW5lMJnL0wjoiiiKSJCEMQ87nM7vdTq68NZ1O5eiFdQRAo9EgTVOCIGCxWLBer+VKIWG9Xv+Rw3eOxyNZltHtdul0OhwOB/b7vVxz5hTBh0KcI/gT0uv1KgkpFEHFIU4fK2mz2TCfz3k8Htzvd3lszel2vFOr1bhcLnJsrdSTMMoEUFVEWT7C8BGGjzB+ARemkU7ZhSRnAAAAAElFTkSuQmCC";
    
    public TradingTechnicControllerTest(
        TradingJournalApiFactory apiFactory,
        ITestOutputHelper testOutputHelper,
        TradingTechnicClassFixture fixture)
    {
        _client = apiFactory.CreateClient();
        _logger = apiFactory.LoggerProvider.Logger;
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
        // Access the ApplicationDbContext through the DI container
        var scope = apiFactory.Services.CreateScope();
        _dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    }
    
    
    #region Create

    [Fact, TestPriority(1)]
    public async Task Create_CreateTradingTechnic_WhenDataIsValid()
    {
        // Arrange
        SeedData();
        var createTradingTechnicCommand = new CreateTradingTechnicCommand
        {
            Name = "name 1",
            Description = "description 1",
            NewImages = new List<IFormFile>
            {
                ConvertBase64ToIFormFile(
                    _image1Base64,
                    "image1.png",
                    "image/png"
                ),
                ConvertBase64ToIFormFile(
                    _image2Base64,
                    "image2.png",
                    "image/png"
                ),
            },
            UserId = Guid.Parse(_fixture.UserId)
        };

        _fixture.CreateTradingTechnicCommand = createTradingTechnicCommand;

        // Act
        var multipartRequestContent = new MultipartFormDataContent();
        multipartRequestContent.Add(new StringContent(createTradingTechnicCommand.Name), "Name");
        multipartRequestContent.Add(new StringContent(createTradingTechnicCommand.Description), "Description");
        multipartRequestContent.Add(new StringContent(createTradingTechnicCommand.UserId.ToString()), "UserId");

        foreach (var file in createTradingTechnicCommand.NewImages)
        {
             var fileContent = new StreamContent(file.OpenReadStream()); 
             fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType); 
             multipartRequestContent.Add(fileContent, "NewImages", file.FileName); 
        }

        var response = await _client.PostAsync(_fixture.Uri, multipartRequestContent);
                

        // Assert
        var tradingTechnciResponse =
            await response.Content.ReadFromJsonAsync<CreateTradingTechnicResult>();
        _fixture.Id = tradingTechnciResponse!.Id;
        tradingTechnciResponse.Should().BeOfType<CreateTradingTechnicResult>();
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location!.ToString().Should()
            .Be($"http://localhost/{_fixture.Uri}/{tradingTechnciResponse.Id}");
    }

    [Fact, TestPriority(2)]
    public async Task Create_ReturnsValidationError_WhenNameIsNullOrEmpty()
    {
        // Arrange
        var createTradingTechnicCommand = new CreateTradingTechnicCommand
        {
            Name = "",
            Description = "description 1",
            NewImages = new List<IFormFile>
            {
                ConvertBase64ToIFormFile(
                    _image1Base64,
                    "image1.png",
                    "image/png"
                ),
                ConvertBase64ToIFormFile(
                    _image2Base64,
                    "image2.png",
                    "image/png"
                ),
            },
            UserId = Guid.Parse(_fixture.UserId)
        };

        // Act
        var multipartRequestContent = new MultipartFormDataContent();
        //multipartRequestContent.Add(new StringContent(createTradingTechnicCommand.Name), "Name");
        multipartRequestContent.Add(new StringContent(createTradingTechnicCommand.Description), "Description");

        foreach (var file in createTradingTechnicCommand.NewImages)
        {
            var fileContent = new StreamContent(file.OpenReadStream()); 
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType); 
            multipartRequestContent.Add(fileContent, "NewImages", file.FileName); 
        }

        var response = await _client.PostAsync(_fixture.Uri, multipartRequestContent);

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
    public async Task Create_ReturnsValidationError_WhenDescriptionIsNullOrEmpty()
    {
        // Arrange
        var createTradingTechnicCommand = new CreateTradingTechnicCommand
        {
            Name = "name 1",
            Description = "",
            NewImages = new List<IFormFile>
            {
                ConvertBase64ToIFormFile(
                    _image1Base64,
                    "image1.png",
                    "image/png"
                ),
                ConvertBase64ToIFormFile(
                    _image2Base64,
                    "image2.png",
                    "image/png"
                ),
            },
            UserId = Guid.Parse(_fixture.UserId)
        };

        // Act
        var multipartRequestContent = new MultipartFormDataContent();
        multipartRequestContent.Add(new StringContent(createTradingTechnicCommand.Name), "Name");
        //multipartRequestContent.Add(new StringContent(createTradingTechnicCommand.Description), "Description");

        foreach (var file in createTradingTechnicCommand.NewImages)
        {
            var fileContent = new StreamContent(file.OpenReadStream()); 
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType); 
            multipartRequestContent.Add(fileContent, "NewImages", file.FileName); 
        }

        var response = await _client.PostAsync(_fixture.Uri, multipartRequestContent);

        // Assert
        var problemDetailsResponse =
            await response.Content.ReadFromJsonAsync<ProblemDetails>();

        problemDetailsResponse.Should().BeOfType<ProblemDetails>();
        problemDetailsResponse!.Status.Should().Be(400);
        problemDetailsResponse.Title.Should().Be("ValidationException");
        problemDetailsResponse.Extensions.Keys.Should().Contain("ValidationErrors");
        problemDetailsResponse.Detail.Should().Contain("Description is required.");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        _logger.LogMessages.Should().ContainMatch("Error Message:*");

        ShowLogs();
    }
    

    #endregion

    #region GetById

    [Fact, TestPriority(5)]
    public async Task GetById_ReturnGetTradingTechnicByIdResult_WhenTradingTechnicExists()
    {
        // Arrange
    
        // Act
        var response = await _client.GetAsync($"{_fixture.Uri}/{_fixture.Id}");
    
        // Assert
        var retrievedTradingTechnic = await response.Content.ReadFromJsonAsync<GetTradingTechnicByIdResult>();
        retrievedTradingTechnic!.Id.Should().Be(_fixture.Id);
        retrievedTradingTechnic.Name.Should().Be(_fixture.CreateTradingTechnicCommand!.Name);
        retrievedTradingTechnic.Description.Should().Be(_fixture.CreateTradingTechnicCommand!.Description);
        retrievedTradingTechnic.Images.Should().HaveCount(2);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact, TestPriority(6)]
    public async Task GetById_ReturnsNotFound_WhenTradingTechnicDoesNotExist()
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

    [Fact, TestPriority(7)]
    public async Task Update_UpdatesTradingTechnic_WhenDataIsValid()
    {
        // Arrange
        
        var tradingTechnic = _dbContext.TradingTechnics.Include(tradingTechnic => tradingTechnic.Images).First(t=>t.Id == TradingTechnicId.Of(_fixture.Id));
        var updateTradingTechnicCommand = new UpdateTradingTechnicCommand
        {
            Id = _fixture.Id,
            Name = "updated name 1",
            Description = "updated description 1",
            Images = [tradingTechnic.Images.ElementAt(0).Path],
            RemovedImages = [tradingTechnic.Images.ElementAt(1).Path],
            NewImages = [
                ConvertBase64ToIFormFile(
                    _image1Base64,
                    "image1.png",
                    "image/png"
                ),
            ]
        };

        _fixture.UpdateTradingTechnicCommand = updateTradingTechnicCommand;

        var multipartRequestContent = new MultipartFormDataContent();
        multipartRequestContent.Add(new StringContent(updateTradingTechnicCommand.Id.ToString()), "Id");
        multipartRequestContent.Add(new StringContent(updateTradingTechnicCommand.Name), "Name");
        multipartRequestContent.Add(new StringContent(updateTradingTechnicCommand.Description), "Description");
        
        foreach (var path in updateTradingTechnicCommand.Images)
        {
            multipartRequestContent.Add(new StringContent(path), "Images"); 
        }
        
        foreach (var path in updateTradingTechnicCommand.RemovedImages)
        {
            multipartRequestContent.Add(new StringContent(path), "RemovedImages"); 
        }
        
        foreach (var file in updateTradingTechnicCommand.NewImages)
        {
            var fileContent = new StreamContent(file.OpenReadStream()); 
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType); 
            multipartRequestContent.Add(fileContent, "NewImages", file.FileName); 
        }
        
        // Act
        var response =
            await _client.PutAsync($"{_fixture.Uri}/{updateTradingTechnicCommand.Id}",
                multipartRequestContent);


        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact, TestPriority(8)]
    public async Task Update_ReturnsValidationError_WhenIdIsNullOrEmpty()
    {
        // Arrange
        var tradingTechnic = _dbContext.TradingTechnics.Include(tradingTechnic => tradingTechnic.Images).First(t=>t.Id == TradingTechnicId.Of(_fixture.Id));
        
        var updateTradingTechnicCommand = new UpdateTradingTechnicCommand
        {
            Id = Guid.Empty,
            Name = "updated name 1",
            Description = "updated description 1",
            Images = [tradingTechnic.Images.ElementAt(1).Path],
            RemovedImages = [tradingTechnic.Images.ElementAt(0).Path]
        };

        var multipartRequestContent = new MultipartFormDataContent();
        multipartRequestContent.Add(new StringContent(updateTradingTechnicCommand.Id.ToString()), "Id");
        multipartRequestContent.Add(new StringContent(updateTradingTechnicCommand.Name), "Name");
        multipartRequestContent.Add(new StringContent(updateTradingTechnicCommand.Description), "Description");
        
        foreach (var path in updateTradingTechnicCommand.Images)
        {
            multipartRequestContent.Add(new StringContent(path), "Images"); 
        }
        
        foreach (var path in updateTradingTechnicCommand.RemovedImages)
        {
            multipartRequestContent.Add(new StringContent(path), "RemovedImages"); 
        }
        
        foreach (var file in updateTradingTechnicCommand.NewImages)
        {
            var fileContent = new StreamContent(file.OpenReadStream()); 
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType); 
            multipartRequestContent.Add(fileContent, "NewImages", file.FileName); 
        }
        
        // Act
        var response =
            await _client.PutAsync($"{_fixture.Uri}/{updateTradingTechnicCommand.Id}",
                multipartRequestContent);


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

    [Fact, TestPriority(9)]
    public async Task Update_ReturnsValidationError_WhenNameIsNullOrEmpty()
    {
        // Arrange
        var tradingTechnic = _dbContext.TradingTechnics.Include(tradingTechnic => tradingTechnic.Images).First(t=>t.Id == TradingTechnicId.Of(_fixture.Id));
        
        var updateTradingTechnicCommand = new UpdateTradingTechnicCommand
        {
            Id = _fixture.Id,
            Name = "",
            Description = "updated description 1",
            Images = [tradingTechnic.Images.ElementAt(1).Path],
            RemovedImages = [tradingTechnic.Images.ElementAt(0).Path]
        };

        var multipartRequestContent = new MultipartFormDataContent();
        multipartRequestContent.Add(new StringContent(updateTradingTechnicCommand.Id.ToString()), "Id");
        //multipartRequestContent.Add(new StringContent(updateTradingTechnicCommand.Name), "Name");
        multipartRequestContent.Add(new StringContent(updateTradingTechnicCommand.Description), "Description");
        
        foreach (var path in updateTradingTechnicCommand.Images)
        {
            multipartRequestContent.Add(new StringContent(path), "Images"); 
        }
        
        foreach (var path in updateTradingTechnicCommand.RemovedImages)
        {
            multipartRequestContent.Add(new StringContent(path), "RemovedImages"); 
        }
        
        foreach (var file in updateTradingTechnicCommand.NewImages)
        {
            var fileContent = new StreamContent(file.OpenReadStream()); 
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType); 
            multipartRequestContent.Add(fileContent, "NewImages", file.FileName); 
        }
        
        // Act
        var response =
            await _client.PutAsync($"{_fixture.Uri}/{updateTradingTechnicCommand.Id}",
                multipartRequestContent);


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

    [Fact, TestPriority(10)]
    public async Task Update_ReturnsValidationError_WhenDescriptionIsNullOrEmpty()
    {
        // Arrange
        var tradingTechnic = _dbContext.TradingTechnics.Include(tradingTechnic => tradingTechnic.Images).First(t=>t.Id == TradingTechnicId.Of(_fixture.Id));
        
        var updateTradingTechnicCommand = new UpdateTradingTechnicCommand
        {
            Id = _fixture.Id,
            Name = "updated name 1",
            Description = "",
            Images = [tradingTechnic.Images.ElementAt(1).Path],
            RemovedImages = [tradingTechnic.Images.ElementAt(0).Path]
        };

        var multipartRequestContent = new MultipartFormDataContent();
        multipartRequestContent.Add(new StringContent(updateTradingTechnicCommand.Id.ToString()), "Id");
        multipartRequestContent.Add(new StringContent(updateTradingTechnicCommand.Name), "Name");
        //multipartRequestContent.Add(new StringContent(updateTradingTechnicCommand.Description), "Description");
        
        foreach (var path in updateTradingTechnicCommand.Images)
        {
            multipartRequestContent.Add(new StringContent(path), "Images"); 
        }
        
        foreach (var path in updateTradingTechnicCommand.RemovedImages)
        {
            multipartRequestContent.Add(new StringContent(path), "RemovedImages"); 
        }
        
        foreach (var file in updateTradingTechnicCommand.NewImages)
        {
            var fileContent = new StreamContent(file.OpenReadStream()); 
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType); 
            multipartRequestContent.Add(fileContent, "NewImages", file.FileName); 
        }
        
        // Act
        var response =
            await _client.PutAsync($"{_fixture.Uri}/{updateTradingTechnicCommand.Id}",
                multipartRequestContent);


        // Assert
        var problemDetailsResponse =
            await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetailsResponse.Should().BeOfType<ProblemDetails>();
        problemDetailsResponse!.Status.Should().Be(400);
        problemDetailsResponse.Title.Should().Be("ValidationException");
        problemDetailsResponse.Extensions.Keys.Should().Contain("ValidationErrors");
        problemDetailsResponse.Detail.Should().Contain("Description is required.");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        _logger.LogMessages.Should().ContainMatch("Error Message:*");

        ShowLogs();
    }

    [Fact, TestPriority(11)]
    public async Task Update_ReturnsArgumentException_WhenImagesHasEmptyString()
    {
        // Arrange
        var updateTradingTechnicCommand = new UpdateTradingTechnicCommand
        {
            Id = _fixture.Id,
            Name = "updated name 1",
            Description = "updated description 1",
            Images = [""],
            RemovedImages = [],
            NewImages = []
        };

        var multipartRequestContent = new MultipartFormDataContent();
        multipartRequestContent.Add(new StringContent(updateTradingTechnicCommand.Id.ToString()), "Id");
        multipartRequestContent.Add(new StringContent(updateTradingTechnicCommand.Name), "Name");
        multipartRequestContent.Add(new StringContent(updateTradingTechnicCommand.Description), "Description");
        
        foreach (var path in updateTradingTechnicCommand.Images)
        {
            multipartRequestContent.Add(new StringContent(path), "Images"); 
        }
        
        foreach (var path in updateTradingTechnicCommand.RemovedImages)
        {
            multipartRequestContent.Add(new StringContent(path), "RemovedImages"); 
        }
        
        foreach (var file in updateTradingTechnicCommand.NewImages)
        {
            var fileContent = new StreamContent(file.OpenReadStream()); 
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType); 
            multipartRequestContent.Add(fileContent, "NewImages", file.FileName); 
        }
        
        // Act
        var response =
            await _client.PutAsync($"{_fixture.Uri}/{updateTradingTechnicCommand.Id}",
                multipartRequestContent);

        // Assert
        var problemDetailsResponse =
            await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetailsResponse.Should().BeOfType<ProblemDetails>();
        problemDetailsResponse!.Status.Should().Be(500);
        problemDetailsResponse.Title.Should().Be("ArgumentNullException");
        problemDetailsResponse.Detail.Should()
            .Contain("Value cannot be null. (Parameter 'path')");
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        _logger.LogMessages.Should().ContainMatch("Error Message:*");

        ShowLogs();
    }

    [Fact, TestPriority(12)]
    public async Task Update_ReturnsArgumentException_WhenRemovedImagesHasEmptyString()
    {
        // Arrange
        var tradingTechnic = _dbContext.TradingTechnics.Include(tradingTechnic => tradingTechnic.Images).First(t=>t.Id == TradingTechnicId.Of(_fixture.Id));
        
        var updateTradingTechnicCommand = new UpdateTradingTechnicCommand
        {
            Id = _fixture.Id,
            Name = "updated name 1",
            Description = "updated description 1",
            Images = [tradingTechnic.Images.ElementAt(1).Path],
            RemovedImages = [""],
            NewImages = []
        };

        var multipartRequestContent = new MultipartFormDataContent();
        multipartRequestContent.Add(new StringContent(updateTradingTechnicCommand.Id.ToString()), "Id");
        multipartRequestContent.Add(new StringContent(updateTradingTechnicCommand.Name), "Name");
        multipartRequestContent.Add(new StringContent(updateTradingTechnicCommand.Description), "Description");
        
        foreach (var path in updateTradingTechnicCommand.Images)
        {
            multipartRequestContent.Add(new StringContent(path), "Images"); 
        }
        
        foreach (var path in updateTradingTechnicCommand.RemovedImages)
        {
            multipartRequestContent.Add(new StringContent(path), "RemovedImages"); 
        }
        
        foreach (var file in updateTradingTechnicCommand.NewImages)
        {
            var fileContent = new StreamContent(file.OpenReadStream()); 
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType); 
            multipartRequestContent.Add(fileContent, "NewImages", file.FileName); 
        }
        
        // Act
        var response =
            await _client.PutAsync($"{_fixture.Uri}/{updateTradingTechnicCommand.Id}",
                multipartRequestContent);


        // Assert
        var problemDetailsResponse =
            await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetailsResponse.Should().BeOfType<ProblemDetails>();
        problemDetailsResponse!.Status.Should().Be(500);
        problemDetailsResponse.Title.Should().Be("ArgumentNullException");
        problemDetailsResponse.Detail.Should()
            .Contain("Value cannot be null. (Parameter 'fileName')");
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        _logger.LogMessages.Should().ContainMatch("Error Message:*");

        ShowLogs();
    }

    [Fact, TestPriority(13)]
    public async Task Update_ReturnsValidationError_WhenImagesHasRemovedImagesIds()
    {
        // Arrange
        var tradingTechnic = _dbContext.TradingTechnics.Include(tradingTechnic => tradingTechnic.Images).First(t=>t.Id == TradingTechnicId.Of(_fixture.Id));
        
        var updateTradingTechnicCommand = new UpdateTradingTechnicCommand
        {
            Id = _fixture.Id,
            Name = "updated name 1",
            Description = "updated description 1",
            Images = [tradingTechnic.Images.ElementAt(1).Path],
            RemovedImages = [tradingTechnic.Images.ElementAt(1).Path]
        };

        var multipartRequestContent = new MultipartFormDataContent();
        multipartRequestContent.Add(new StringContent(updateTradingTechnicCommand.Id.ToString()), "Id");
        multipartRequestContent.Add(new StringContent(updateTradingTechnicCommand.Name), "Name");
        multipartRequestContent.Add(new StringContent(updateTradingTechnicCommand.Description), "Description");
        
        foreach (var path in updateTradingTechnicCommand.Images)
        {
            multipartRequestContent.Add(new StringContent(path), "Images"); 
        }
        
        foreach (var path in updateTradingTechnicCommand.RemovedImages)
        {
            multipartRequestContent.Add(new StringContent(path), "RemovedImages"); 
        }
        
        foreach (var file in updateTradingTechnicCommand.NewImages)
        {
            var fileContent = new StreamContent(file.OpenReadStream()); 
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType); 
            multipartRequestContent.Add(fileContent, "NewImages", file.FileName); 
        }
        
        // Act
        var response =
            await _client.PutAsync($"{_fixture.Uri}/{updateTradingTechnicCommand.Id}",
                multipartRequestContent);


        // Assert
        var problemDetailsResponse =
            await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetailsResponse.Should().BeOfType<ProblemDetails>();
        problemDetailsResponse!.Status.Should().Be(400);
        problemDetailsResponse.Title.Should().Be("ValidationException");
        problemDetailsResponse.Extensions.Keys.Should().Contain("ValidationErrors");
        problemDetailsResponse.Detail.Should()
            .Contain("The Images contains values that are also present in the RemovedImages.");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        _logger.LogMessages.Should().ContainMatch("Error Message:*");

        ShowLogs();
    }

    [Fact, TestPriority(14)]
    public async Task Update_ReturnsBadRequest_WhenIdIsInvalid()
    {
        // Arrange
        var updateTradingTechnicCommand = _fixture.UpdateTradingTechnicCommand;
        var multipartRequestContent = new MultipartFormDataContent();
        multipartRequestContent.Add(new StringContent(updateTradingTechnicCommand!.Id.ToString()), "Id");
        multipartRequestContent.Add(new StringContent(updateTradingTechnicCommand.Name), "Name");
        multipartRequestContent.Add(new StringContent(updateTradingTechnicCommand.Description), "Description");
        
        foreach (var path in updateTradingTechnicCommand.Images)
        {
            multipartRequestContent.Add(new StringContent(path), "Images"); 
        }
        
        foreach (var path in updateTradingTechnicCommand.RemovedImages)
        {
            multipartRequestContent.Add(new StringContent(path), "RemovedImages"); 
        }
        
        foreach (var file in updateTradingTechnicCommand.NewImages)
        {
            var fileContent = new StreamContent(file.OpenReadStream()); 
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType); 
            multipartRequestContent.Add(fileContent, "NewImages", file.FileName); 
        }
        
        // Act
        var response =
            await _client.PutAsync($"{_fixture.Uri}/{Guid.NewGuid()}",
                multipartRequestContent);
        
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

    [Fact, TestPriority(15)]
    public async Task GetByName_ReturnGetTradingTechnicByNameResultList_WhenTradingTechnicExists()
    {
        // Arrange
        var getTradingTechnicByNameResult = new GetTradingTechnicByNameResult
        {
            Id = _fixture.Id,
            Name = _fixture.UpdateTradingTechnicCommand!.Name,
            Description = _fixture.UpdateTradingTechnicCommand!.Description,
            Images = _fixture.UpdateTradingTechnicCommand!.Images,
        };

        // Act
        var response = await _client.GetAsync($"{_fixture.Uri}/{_fixture.UpdateTradingTechnicCommand!.Name}");

        // Assert
        var retrievedTradingTechnic =
            await response.Content.ReadFromJsonAsync<ICollection<GetTradingTechnicByNameResult>>();
        retrievedTradingTechnic!.Should().BeEquivalentTo([getTradingTechnicByNameResult], options =>
        {
            //options.Excluding(x => x.Images);
            options.Using<ICollection<string>>(ctx => { ctx.Subject.Count.Should().Be(2); })
                .When(info => info.Path.EndsWith("Images"));
            return options;
        });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact, TestPriority(16)]
    public async Task GetByName_ReturnsEmptyList_WhenTradingTechnicDoesNotExist()
    {
        // Act
        var response = await _client.GetAsync($"{_fixture.Uri}/Abc");

        // Assert

        var retrievedTradingTechnic =
            await response.Content.ReadFromJsonAsync<ICollection<GetTradingTechnicByNameResult>>();
        retrievedTradingTechnic!.Should().BeEquivalentTo(new List<GetTradingTechnicByNameResult>());
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion

    #region Delete

    [Fact, TestPriority(17)]
    public async Task Delete_DeleteTradingTechnic_WhenTradingTechnicExists()
    {
        // Arrange

        // Act
        var response = await _client.DeleteAsync($"{_fixture.Uri}/{_fixture.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact, TestPriority(18)]
    public async Task Delete_ReturnsNotFound_WhenTradingTechnicDoesNotExist()
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

    [Fact, TestPriority(19)]
    public async Task GetAll_ReturnsEmptyResult_WhenNoTradingTechnicsExistWithDefaultQuery()
    {
        // Arrange
        var paginatedResult = new PaginatedResult<GetTradingTechnicsResult>([],0, 1, 5);

        // Act
        var response = await _client.GetAsync($"{_fixture.Uri}?pageSize=5&pageNumber=1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var tradingTechnicsResponse =
            await response.Content.ReadFromJsonAsync<PaginatedResult<GetTradingTechnicsResult>>();
        tradingTechnicsResponse.Should().BeOfType<PaginatedResult<GetTradingTechnicsResult>>();
        tradingTechnicsResponse!.Should().BeEquivalentTo(paginatedResult);
    }

    [Fact, TestPriority(20)]
    public async Task GetAll_ReturnsFirstFiveTradingTechnicsOfNineteen_WhenPageNumberOneAndPageSizeFive()
    {
        // Arrange
        var getTradingTechnicsResult = new List<GetTradingTechnicsResult>();
    
        for (var i = 1; i <= 19; i++)
        {
            var createTradingTechnicCommand = new CreateTradingTechnicCommand
            {
                Name = $"name {i}",
                Description = $"description {i}",
                NewImages =  new List<IFormFile>
                {
                    ConvertBase64ToIFormFile(
                        _image1Base64,
                        $"image{i}.png",
                        "image/png"
                    ),
                },
                UserId = Guid.Parse(_fixture.UserId)
            };
            var multipartRequestContent = new MultipartFormDataContent();
            multipartRequestContent.Add(new StringContent(createTradingTechnicCommand.Name), "Name");
            multipartRequestContent.Add(new StringContent(createTradingTechnicCommand.Description), "Description");
            multipartRequestContent.Add(new StringContent(createTradingTechnicCommand.UserId.ToString()), "UserId");

            foreach (var file in createTradingTechnicCommand.NewImages)
            {
                var fileContent = new StreamContent(file.OpenReadStream()); 
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType); 
                multipartRequestContent.Add(fileContent, "NewImages", file.FileName); 
            }
            var createResponse = await _client.PostAsync(_fixture.Uri, multipartRequestContent); 
            var createResult = await createResponse.Content.ReadFromJsonAsync<CreateTradingTechnicResult>();
    
            getTradingTechnicsResult.Add(new GetTradingTechnicsResult
            {
                Id = createResult!.Id,
                Name = createTradingTechnicCommand.Name,
                Description = createTradingTechnicCommand.Description
            });
        }
    
        _fixture.GetTradingTechnicsResultList = getTradingTechnicsResult.ToList();
    
        var paginatedResult =
            new PaginatedResult<GetTradingTechnicsResult>(getTradingTechnicsResult.Skip(0).Take(5).ToList(),19, 1, 5 );
    
    
        // Act
        var response = await _client.GetAsync($"{_fixture.Uri}?pageSize=5&pageNumber=1");
    
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var tradingTechnicsResponse =
            await response.Content.ReadFromJsonAsync<PaginatedResult<GetTradingTechnicsResult>>();
        tradingTechnicsResponse.Should().BeOfType<PaginatedResult<GetTradingTechnicsResult>>();
        tradingTechnicsResponse!.Data.Should().HaveCount(5);
        tradingTechnicsResponse.Should().BeEquivalentTo(paginatedResult,options =>
        {
            //options.Excluding(x => x.Images);
            options.Using<ICollection<string>>(ctx => { ctx.Subject.Count.Should().Be(1); })
                .When(info => info.Path.EndsWith("Images"));
            return options;
        });
    }

    [Fact, TestPriority(21)]
    public async Task GetAll_ReturnsLastFourTradingTechnicsOfNineteen_WhenPageNumberFourAndPageSizeFive()
    {
        // Arrange
        var paginatedResult =
            new PaginatedResult<GetTradingTechnicsResult>(_fixture.GetTradingTechnicsResultList.Skip(15).Take(5).ToList(),19, 4, 5);

        // Act
        var response = await _client.GetAsync($"{_fixture.Uri}?pageSize=5&pageNumber=4");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var tradingTechnicsResponse =
            await response.Content.ReadFromJsonAsync<PaginatedResult<GetTradingTechnicsResult>>();
        tradingTechnicsResponse.Should().BeOfType<PaginatedResult<GetTradingTechnicsResult>>();
        tradingTechnicsResponse!.Data.Should().HaveCount(4);
        tradingTechnicsResponse.Should().BeEquivalentTo(paginatedResult,options =>
        {
            //options.Excluding(x => x.Images);
            options.Using<ICollection<string>>(ctx => { ctx.Subject.Count.Should().Be(1); })
                .When(info => info.Path.EndsWith("Images"));
            return options;
        });
    }

    [Fact, TestPriority(22)]
    public async Task GetAll_ReturnsFirstFiveTradingTechnics_WhenTradingTechnicsExistWithSearchQuery()
    {
        // Arrange
        var paginatedResult = new PaginatedResult<GetTradingTechnicsResult>(
            _fixture.GetTradingTechnicsResultList.Where(x => x.Name.Contains("e 1")).Skip(0).Take(5).ToList(),11, 1, 5);

        // Act
        var response = await _client.GetAsync($"{_fixture.Uri}?pageSize=5&pageNumber=1&search=e 1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var tradingTechnicsResponse =
            await response.Content.ReadFromJsonAsync<PaginatedResult<GetTradingTechnicsResult>>();
        tradingTechnicsResponse.Should().BeOfType<PaginatedResult<GetTradingTechnicsResult>>();
        tradingTechnicsResponse!.Data.Should().HaveCount(5);
        tradingTechnicsResponse.Should().BeEquivalentTo(paginatedResult,options =>
        {
            //options.Excluding(x => x.Images);
            options.Using<ICollection<string>>(ctx => { ctx.Subject.Count.Should().Be(1); })
                .When(info => info.Path.EndsWith("Images"));
            return options;
        });
    }

    [Fact, TestPriority(23)]
    public async Task GetAll_ReturnsEmptyResult_WhenTradingTechnicsNotExistWithSearchQuery()
    {
        // Arrange
        var paginatedResult = new PaginatedResult<GetTradingTechnicsResult>([],0, 1, 5);

        // Act
        var response = await _client.GetAsync($"{_fixture.Uri}?pageSize=5&pageNumber=1&search=abcd");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var tradingTechnicsResponse =
            await response.Content.ReadFromJsonAsync<PaginatedResult<GetTradingTechnicsResult>>();
        tradingTechnicsResponse.Should().BeOfType<PaginatedResult<GetTradingTechnicsResult>>();
        tradingTechnicsResponse!.Data.Should().HaveCount(0);
        tradingTechnicsResponse.Should().BeEquivalentTo(paginatedResult);
    }

    [Fact, TestPriority(24)]
    public async Task GetAll_ReturnsFirstFiveAscOrderedTradingTechnicOfNineteen_WhenOrderAscQueryValid()
    {
        // Arrange
        var paginatedResult = new PaginatedResult<GetTradingTechnicsResult>(
            _fixture.GetTradingTechnicsResultList.OrderBy(x => x.Name).Skip(0).Take(5).ToList(),19, 1, 5);

        // Act
        var response = await _client.GetAsync($"{_fixture.Uri}?pageSize=5&pageNumber=1&search=&sorts[0].sortBy=Name&sorts[0].sortOrder=asc&sorts[0].order=1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var tradingTechnicsResponse =
            await response.Content.ReadFromJsonAsync<PaginatedResult<GetTradingTechnicsResult>>();
        tradingTechnicsResponse.Should().BeOfType<PaginatedResult<GetTradingTechnicsResult>>();
        tradingTechnicsResponse!.Data.Should().HaveCount(5);
        tradingTechnicsResponse.Should().BeEquivalentTo(paginatedResult,options =>
        {
            //options.Excluding(x => x.Images);
            options.Using<ICollection<string>>(ctx => { ctx.Subject.Count.Should().Be(1); })
                .When(info => info.Path.EndsWith("Images"));
            return options;
        });
    }

    [Fact, TestPriority(25)]
    public async Task GetAll_ReturnsFirstFiveDescOrderedTradingTechnicOfNineteen_WhenOrderDescQueryValid()
    {
        // Arrange
        var paginatedResult = new PaginatedResult<GetTradingTechnicsResult>(
            _fixture.GetTradingTechnicsResultList.OrderByDescending(x => x.Name).Skip(0).Take(5).ToList(),19, 1, 5);

        // Act
        var response = await _client.GetAsync($"{_fixture.Uri}?pageSize=5&pageNumber=1&search=&sorts[0].sortBy=Name&sorts[0].sortOrder=desc&sorts[0].order=1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var tradingTechnicsResponse =
            await response.Content.ReadFromJsonAsync<PaginatedResult<GetTradingTechnicsResult>>();
        tradingTechnicsResponse.Should().BeOfType<PaginatedResult<GetTradingTechnicsResult>>();
        tradingTechnicsResponse!.Data.Should().HaveCount(5);
        tradingTechnicsResponse.Should().BeEquivalentTo(paginatedResult,options =>
        {
            //options.Excluding(x => x.Images);
            options.Using<ICollection<string>>(ctx => { ctx.Subject.Count.Should().Be(1); })
                .When(info => info.Path.EndsWith("Images"));
            return options;
        });
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
        
        _dbContext.SaveChanges();
    }
    private IFormFile ConvertBase64ToIFormFile(string base64String, string fileName, string contentType)
    {
        // مرحله 1: حذف پیشوند Base64 (اگر وجود داشته باشد)
        if (base64String.StartsWith("data:image"))
        {
            var index = base64String.IndexOf(",");
            if (index != -1)
            {
                base64String = base64String.Substring(index + 1);
            }
        }

        // مرحله 2: تبدیل Base64 به byte[]
        byte[] fileBytes = Convert.FromBase64String(base64String);

        // مرحله 3: ایجاد یک MemoryStream از byte[]
        var stream = new MemoryStream(fileBytes);

        // مرحله 4: ساخت IFormFile از MemoryStream
        IFormFile formFile = new FormFile(stream, 0, stream.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType
        };

        return formFile;
    }
}