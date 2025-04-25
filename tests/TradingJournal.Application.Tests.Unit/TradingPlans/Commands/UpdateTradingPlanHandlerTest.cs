using BuildingBlocks.Exceptions;
using FluentAssertions;
using NSubstitute;
using TradingJournal.Application.Repositories;
using TradingJournal.Application.TradingPlans.Commands.UpdateTradingPlan;
using TradingJournal.Domain.Models;
using TradingJournal.Domain.ValueObjects;

namespace TradingJournal.Application.Tests.Unit.TradingPlans.Commands;

public class UpdateTradingPlanHandlerTest
{
    private readonly UpdateTradingPlanHandler _sut;
    private readonly ITradingPlanRepository _repo = Substitute.For<ITradingPlanRepository>();
    private readonly UserId _userId;
    
    public UpdateTradingPlanHandlerTest()
    {
        _sut = new UpdateTradingPlanHandler(_repo);
        _userId = UserId.Of(Guid.Parse("611C9504-8536-48E0-8656-C55989001A44"));
    }

    [Fact]
    public async Task UpdateHandle_ReturnTrue_WhenTradingPlanValid()
    {
        // Arrange
        var updateTradingPlanCommand = new UpdateTradingPlanCommand()
        {
            Id = Guid.Parse("3B0A0F86-E336-42E2-8FA8-A3388204B3C7"),
            Name = "Test Name",
            Technics = new List<Guid>()
            {
                Guid.Parse("3B0A0F86-E336-42E2-8FA8-A3388204B3C4"),
                Guid.Parse("3B0A0F86-E336-42E2-8FA8-A3388204B3C5"),
                Guid.Parse("3B0A0F86-E336-42E2-8FA8-A3388204B3C0")
            },
            FromTime = new TimeOnly(00, 00, 00),
            ToTime = new TimeOnly(21, 30, 00),
            SelectedDays = new List<string>() { "Monday", "Tuesday", "Wednesday", "Thursday" },
            RemovedTechnics = [Guid.Parse("3B0A0F86-E336-42E2-8FA8-A3388204B3C5")]
        };
        var technics = new List<TradingTechnic>
        {
            TradingTechnic.Create(TradingTechnicId.Of(updateTradingPlanCommand.Technics[0]), "Name Technic 1",
                "Description Technic 1",_userId),
            TradingTechnic.Create(TradingTechnicId.Of(updateTradingPlanCommand.Technics[1]), "Name Technic 2",
                "Description Technic 2",_userId),
            TradingTechnic.Create(TradingTechnicId.Of(updateTradingPlanCommand.Technics[2]), "Name Technic 3",
                "Description Technic 3",_userId),
        };

        var tradingPlan = TradingPlan.Create(
            TradingPlanId.Of(updateTradingPlanCommand.Id),
            updateTradingPlanCommand.Name,
            updateTradingPlanCommand.FromTime,
            updateTradingPlanCommand.ToTime,
            updateTradingPlanCommand.SelectedDays,
            _userId
        );

        foreach (var technic in technics)
        {
            tradingPlan.AddTechnic(technic);
        }
        

        _repo.GetByIdAsync(TradingPlanId.Of(updateTradingPlanCommand.Id), Arg.Any<CancellationToken>()).Returns(tradingPlan);
        _repo.GetTechnicsByIdsAsync(updateTradingPlanCommand.Technics, Arg.Any<CancellationToken>())
            .Returns(technics);
        _repo.UpdateAsync(Arg.Any<TradingPlan>(), Arg.Any<CancellationToken>()).Returns(true);

        // Act
        var result = await _sut.Handle(updateTradingPlanCommand, CancellationToken.None);


        // Assert
        result.Should().BeOfType<UpdateTradingPlanResult>();
        result.IsSuccess.Should().BeTrue();
        await _repo.Received(1)
            .GetTechnicsByIdsAsync(
                Arg.Is<IList<Guid>>(a => a.All(technicId => updateTradingPlanCommand.Technics.Contains(technicId))),
                Arg.Any<CancellationToken>());
        await _repo.Received(1).UpdateAsync(Arg.Any<TradingPlan>(), Arg.Any<CancellationToken>());
        await _repo.Received(1).GetByIdAsync(Arg.Any<TradingPlanId>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateHandle_ThrowInternalServerException_WhenStatusFalse()
    {
        // Arrange
        var updateTradingPlanCommand = new UpdateTradingPlanCommand()
        {
            Id = Guid.Parse("3B0A0F86-E336-42E2-8FA8-A3388204B3C7"),
            Name = "Test Name",
            Technics = new List<Guid>()
            {
                Guid.Parse("3B0A0F86-E336-42E2-8FA8-A3388204B3C4"),
                Guid.Parse("3B0A0F86-E336-42E2-8FA8-A3388204B3C5"),
                Guid.Parse("3B0A0F86-E336-42E2-8FA8-A3388204B3C0")
            },
            FromTime = new TimeOnly(00, 00, 00),
            ToTime = new TimeOnly(21, 30, 00),
            SelectedDays = new List<string>() { "Monday", "Tuesday", "Wednesday", "Thursday" },
            RemovedTechnics = [Guid.Parse("3B0A0F86-E336-42E2-8FA8-A3388204B3C5")]
        };
        var technics = new List<TradingTechnic>
        {
            TradingTechnic.Create(TradingTechnicId.Of(updateTradingPlanCommand.Technics[0]), "Name Technic 1",
                "Description Technic 1",_userId),
            TradingTechnic.Create(TradingTechnicId.Of(updateTradingPlanCommand.Technics[1]), "Name Technic 2",
                "Description Technic 2",_userId),
            TradingTechnic.Create(TradingTechnicId.Of(updateTradingPlanCommand.Technics[2]), "Name Technic 3",
                "Description Technic 3",_userId),
        };

        var tradingPlan = TradingPlan.Create(
            TradingPlanId.Of(updateTradingPlanCommand.Id),
            updateTradingPlanCommand.Name,
            updateTradingPlanCommand.FromTime,
            updateTradingPlanCommand.ToTime,
            updateTradingPlanCommand.SelectedDays,
            _userId
        );

        foreach (var technic in technics)
        {
            tradingPlan.AddTechnic(technic);
        }
        

        _repo.GetByIdAsync(TradingPlanId.Of(updateTradingPlanCommand.Id), Arg.Any<CancellationToken>()).Returns(tradingPlan);
        _repo.GetTechnicsByIdsAsync(updateTradingPlanCommand.Technics, Arg.Any<CancellationToken>())
            .Returns(technics);
        _repo.UpdateAsync(Arg.Any<TradingPlan>(), Arg.Any<CancellationToken>()).Returns(false);

        // Act
        var requestAction = async () =>
            await _sut.Handle(updateTradingPlanCommand, CancellationToken.None);

        // Assert
        await requestAction.Should()
            .ThrowAsync<InternalServerException>();
    }

    [Fact]
    public async Task UpdateHandle_ThrowNotFoundException_WhenTradingPlanIdIsInvalid()
    {
        // Arrange
        var updateTradingPlanCommand = new UpdateTradingPlanCommand()
        {
            Id = Guid.Parse("3B0A0F86-E336-42E2-8FA8-A3388204B3C7"),
            Name = "Test Name",
            Technics = new List<Guid>()
            {
                Guid.Parse("3B0A0F86-E336-42E2-8FA8-A3388204B3C4"),
                Guid.Parse("3B0A0F86-E336-42E2-8FA8-A3388204B3C5"),
                Guid.Parse("3B0A0F86-E336-42E2-8FA8-A3388204B3C0")
            },
            FromTime = new TimeOnly(00, 00, 00),
            ToTime = new TimeOnly(21, 30, 00),
            SelectedDays = new List<string>() { "Monday", "Tuesday", "Wednesday", "Thursday" },
            RemovedTechnics = [Guid.Parse("3B0A0F86-E336-42E2-8FA8-A3388204B3C5")]
        };
        var technics = new List<TradingTechnic>
        {
            TradingTechnic.Create(TradingTechnicId.Of(updateTradingPlanCommand.Technics[0]), "Name Technic 1",
                "Description Technic 1",_userId),
            TradingTechnic.Create(TradingTechnicId.Of(updateTradingPlanCommand.Technics[1]), "Name Technic 2",
                "Description Technic 2",_userId),
            TradingTechnic.Create(TradingTechnicId.Of(updateTradingPlanCommand.Technics[2]), "Name Technic 3",
                "Description Technic 3",_userId),
        };
        

        _repo.GetByIdAsync(TradingPlanId.Of(updateTradingPlanCommand.Id), Arg.Any<CancellationToken>()).Returns((TradingPlan?)null);
        _repo.GetTechnicsByIdsAsync(updateTradingPlanCommand.Technics, Arg.Any<CancellationToken>())
            .Returns(technics);
        _repo.UpdateAsync(Arg.Any<TradingPlan>(), Arg.Any<CancellationToken>()).Returns(true);

        // Act
        var requestAction = async () =>
            await _sut.Handle(updateTradingPlanCommand, CancellationToken.None);

        // Assert
        await requestAction.Should()
            .ThrowAsync<NotFoundException>();
    }
}