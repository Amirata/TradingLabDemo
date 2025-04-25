using BuildingBlocks.Exceptions;
using FluentAssertions;
using NSubstitute;
using TradingJournal.Application.Repositories;
using TradingJournal.Application.TradingPlans.Queries.GetTradingPlanById;
using TradingJournal.Application.TradingTechnics.Queries.GetTradingTechnicById;

namespace TradingJournal.Application.Tests.Unit.TradingPlans.Queries;

public class GetTradingPlanByIdHandlerTest
{
    private readonly GetTradingPlanByIdHandler _sut;
    private readonly ITradingPlanRepository _repo = Substitute.For<ITradingPlanRepository>();

    public GetTradingPlanByIdHandlerTest()
    {
        _sut = new GetTradingPlanByIdHandler(_repo);
    }
    
    [Fact]
    public async Task GetTradingPlanByIdHandle_ReturnGetTradingPlanByIdResult_WhenTradingPlanIdValid()
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
        var getTradingPlanByIdResult = new GetTradingPlanByIdResult()
        {
            Id = Guid.Parse("3B0A0F86-E336-42E2-8FA8-A3388204B3C4"),
            Name = "Test Name",
            Technics = technics,
            FromTime = new TimeOnly(00, 00, 00),
            ToTime = new TimeOnly(21, 30, 00),
            SelectedDays = new List<string>() { "Monday", "Tuesday", "Wednesday", "Thursday" }
        };
        
        _repo.GetTradingPlanByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(getTradingPlanByIdResult);
       
        // Act
        var result = await _sut.Handle(new GetTradingPlanByIdQuery
        {
            TradingPlanId = getTradingPlanByIdResult.Id
        }, CancellationToken.None);


        // Assert
        result.Should().BeOfType<GetTradingPlanByIdResult>();
        result.Should().BeEquivalentTo(getTradingPlanByIdResult);
        await _repo.Received(1)
            .GetTradingPlanByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        }
    
    [Fact]
    public async Task GetTradingPlanByIdHandle_ThrowNotFoundException_WhenTradingPlanIdIsInValid()
    {
        // Arrange
        _repo.GetTradingPlanByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((GetTradingPlanByIdResult?)null);
       
        // Act
        var requestAction = async () => await _sut.Handle(new GetTradingPlanByIdQuery
        {
            TradingPlanId = Guid.NewGuid()
        }, CancellationToken.None);

        
        // Assert
        await requestAction.Should()
            .ThrowAsync<NotFoundException>();}
}