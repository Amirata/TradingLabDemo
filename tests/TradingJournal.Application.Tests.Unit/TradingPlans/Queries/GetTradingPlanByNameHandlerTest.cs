using BuildingBlocks.Exceptions;
using FluentAssertions;
using NSubstitute;
using TradingJournal.Application.Repositories;
using TradingJournal.Application.TradingPlans.Queries.GetTradingPlanByName;
using TradingJournal.Application.TradingTechnics.Queries.GetTradingTechnicById;

namespace TradingJournal.Application.Tests.Unit.TradingPlans.Queries;

public class GetTradingPlanByNameHandlerTest
{
    private readonly GetTradingPlanByNameHandler _sut;
    private readonly ITradingPlanRepository _repo = Substitute.For<ITradingPlanRepository>();

    public GetTradingPlanByNameHandlerTest()
    {
        _sut = new GetTradingPlanByNameHandler(_repo);
    }

    [Fact]
    public async Task GetTradingPlanByNameHandle_ReturnGetTradingPlanByNameResult_WhenTradingPlanNameValid()
    {
        // Arrange
        var technics = new List<GetTradingTechnicByIdResult>
        {
            new GetTradingTechnicByIdResult()
            {
                Id = Guid.NewGuid(),
                Name = "Technic 1",
                Description = "Technic 1 description",
            },
            new GetTradingTechnicByIdResult()
            {
                Id = Guid.NewGuid(),
                Name = "Technic 2",
                Description = "Technic 2 description",
            },
           
        };
        var getTradingPlanByNameResult = new GetTradingPlanByNameResult()
        {
            Id = Guid.Parse("3B0A0F86-E336-42E2-8FA8-A3388204B3C4"),
            Name = "Test Name",
            Technics = technics,
            FromTime = new TimeOnly(00, 00, 00),
            ToTime = new TimeOnly(21, 30, 00),
            SelectedDays = new List<string>() { "Monday", "Tuesday", "Wednesday", "Thursday" }
        };

        _repo.GetTradingPlanByNameAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns([getTradingPlanByNameResult]);

        // Act
        var result = await _sut.Handle(new GetTradingPlanByNameQuery
        {
            Name = getTradingPlanByNameResult.Name
        }, CancellationToken.None);


        // Assert
        result.Should().BeOfType<List<GetTradingPlanByNameResult>>();
        result.Should().BeEquivalentTo([getTradingPlanByNameResult]);
        await _repo.Received(1)
            .GetTradingPlanByNameAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetTradingPlanByNameHandle_ReturnEmptyResult_WhenTradingPlanNameIsNotExists()
    {
        // Arrange
        _repo.GetTradingPlanByNameAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns([]);

        // Act
        var result = await _sut.Handle(new GetTradingPlanByNameQuery
            {
                Name = "Test Name"
            }, CancellationToken.None);
        
        // Assert
        result.Should().BeOfType<List<GetTradingPlanByNameResult>>();
        result.Should().BeEquivalentTo(new List<GetTradingPlanByNameResult>());
    }
}