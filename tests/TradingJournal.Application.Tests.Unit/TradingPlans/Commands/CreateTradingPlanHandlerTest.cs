using BuildingBlocks.Exceptions;
using FluentAssertions;
using NSubstitute;
using TradingJournal.Application.Repositories;
using TradingJournal.Application.TradingPlans.Commands.CreateTradingPlan;
using TradingJournal.Domain.Models;
using TradingJournal.Domain.ValueObjects;

namespace TradingJournal.Application.Tests.Unit.TradingPlans.Commands;

public class CreateTradingPlanHandlerTest
{
    private readonly CreateTradingPlanHandler _sut;
    private readonly ITradingPlanRepository _repo = Substitute.For<ITradingPlanRepository>();
    private readonly UserId _userId;

    public CreateTradingPlanHandlerTest()
    {
        _sut = new CreateTradingPlanHandler(_repo);
        _userId = UserId.Of(Guid.Parse("611C9504-8536-48E0-8656-C55989001A44"));
    }

    [Fact]
    public async Task CreateHandle_ReturnTradingPlanId_WhenTradingPlanDtoValid()
    {
        // Arrange
        var createTradingPlanCommand = new CreateTradingPlanCommand()
        {
            Name = "Test Name",
            Technics = new List<Guid>()
            {
                Guid.Parse("3B0A0F86-E336-42E2-8FA8-A3388204B3C4"), Guid.Parse("3B0A0F86-E336-42E2-8FA8-A3388204B3C5")
            },
            FromTime = new TimeOnly(00, 00, 00),
            ToTime = new TimeOnly(21, 30, 00),
            SelectedDays = new List<string>() { "Monday", "Tuesday", "Wednesday", "Thursday" },
            UserId = Guid.Parse("611C9504-8536-48E0-8656-C55989001A44")
        };

        var technics = new List<TradingTechnic>
        {
            TradingTechnic.Create(TradingTechnicId.Of(createTradingPlanCommand.Technics[0]), "Name Technic 1",
                "Description Technic 1",_userId),
            TradingTechnic.Create(TradingTechnicId.Of(createTradingPlanCommand.Technics[1]), "Name Technic 2",
                "Description Technic 2",_userId),
        };

        _repo.GetTechnicsByIdsAsync(createTradingPlanCommand.Technics, Arg.Any<CancellationToken>())
            .Returns(technics);
        _repo.CreateAsync(Arg.Any<TradingPlan>(), Arg.Any<CancellationToken>()).Returns(true);

        // Act
        var result = await _sut.Handle(createTradingPlanCommand, CancellationToken.None);


        // Assert
        result.Should().BeOfType<CreateTradingPlanResult>();
        result.Id.Should().NotBeEmpty();
        await _repo.Received(1)
            .GetTechnicsByIdsAsync(
                Arg.Is<IList<Guid>>(a => a.All(technicId => createTradingPlanCommand.Technics.Contains(technicId))),
                Arg.Any<CancellationToken>());
        await _repo.Received(1).CreateAsync(Arg.Any<TradingPlan>(), Arg.Any<CancellationToken>());
    }
    
    [Fact]
    public async Task CreateHandle_ThrowInternalServerException_WhenStatusFalse()
    {
        // Arrange
        var createTradingPlanCommand = new CreateTradingPlanCommand()
        {
            Name = "Test Name",
            Technics = new List<Guid>()
            {
                Guid.Parse("3B0A0F86-E336-42E2-8FA8-A3388204B3C4"), Guid.Parse("3B0A0F86-E336-42E2-8FA8-A3388204B3C5")
            },
            FromTime = new TimeOnly(00, 00, 00),
            ToTime = new TimeOnly(21, 30, 00),
            SelectedDays = new List<string>() { "Monday", "Tuesday", "Wednesday", "Thursday" },
            UserId = Guid.Parse("611C9504-8536-48E0-8656-C55989001A44")
        };

        var technics = new List<TradingTechnic>
        {
            TradingTechnic.Create(TradingTechnicId.Of(createTradingPlanCommand.Technics[0]), "Name Technic 1",
                "Description Technic 1",_userId),
            TradingTechnic.Create(TradingTechnicId.Of(createTradingPlanCommand.Technics[1]), "Name Technic 2",
                "Description Technic 2",_userId),
        };

        _repo.GetTechnicsByIdsAsync(createTradingPlanCommand.Technics, Arg.Any<CancellationToken>())
            .Returns(technics);
        _repo.CreateAsync(Arg.Any<TradingPlan>(), Arg.Any<CancellationToken>()).Returns(false);

        // Act
        var requestAction = async () => await _sut.Handle(createTradingPlanCommand, CancellationToken.None);

        // Assert
        await requestAction.Should()
            .ThrowAsync<InternalServerException>();
    }
    
    [Fact]
    public async Task CreateHandle_ThrowBadRequestException_WhenTradingTechnicIdIsInvalid()
    {
        // Arrange
        var createTradingPlanCommand = new CreateTradingPlanCommand()
        {
            Name = "Test Name",
            Technics = new List<Guid>()
            {
                Guid.Parse("3B0A0F86-E336-42E2-8FA8-A3388204B3C4"), Guid.Parse("3B0A0F86-E336-42E2-8FA8-A3388204B3C5")
            },
            FromTime = new TimeOnly(00, 00, 00),
            ToTime = new TimeOnly(21, 30, 00),
            SelectedDays = new List<string>() { "Monday", "Tuesday", "Wednesday", "Thursday" },
            UserId = Guid.Parse("611C9504-8536-48E0-8656-C55989001A44")
        };

        var technics = new List<TradingTechnic>();
        

        _repo.GetTechnicsByIdsAsync(createTradingPlanCommand.Technics, Arg.Any<CancellationToken>())
            .Returns(technics);
        _repo.CreateAsync(Arg.Any<TradingPlan>(), Arg.Any<CancellationToken>()).Returns(false);

        // Act
        var requestAction = async () => await _sut.Handle(createTradingPlanCommand, CancellationToken.None);

        // Assert
        await requestAction.Should()
            .ThrowAsync<BadRequestException>();
    }
}