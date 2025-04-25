using BuildingBlocks.Exceptions;
using FluentAssertions;
using NSubstitute;
using TradingJournal.Application.Repositories;
using TradingJournal.Application.TradingPlans.Commands.DeleteTradingPlan;
using TradingJournal.Domain.Models;
using TradingJournal.Domain.ValueObjects;

namespace TradingJournal.Application.Tests.Unit.TradingPlans.Commands;

public class DeleteTradingPlanHandlerTest
{
    private readonly DeleteTradingPlanHandler _sut;
    private readonly ITradingPlanRepository _repo = Substitute.For<ITradingPlanRepository>();
    private readonly UserId _userId;
    public DeleteTradingPlanHandlerTest()
    {
        _sut = new DeleteTradingPlanHandler(_repo);
        _userId = UserId.Of(Guid.Parse("611C9504-8536-48E0-8656-C55989001A44"));
    }

    [Fact]
    public async Task DeleteHandle_ReturnTure_WhenSuccessfullyDelete()
    {
        // Arrange
        var tradingPlanId = Guid.NewGuid();
        var tradingPlan = TradingPlan.Create(TradingPlanId.Of(tradingPlanId), "Test", null, null, [],_userId);
        
        _repo.GetByIdAsync(tradingPlan.Id, Arg.Any<CancellationToken>())
            .Returns(tradingPlan);
        _repo.DeleteAsync(Arg.Any<TradingPlan>(), Arg.Any<CancellationToken>()).Returns(true);

        // Act
        var result = await _sut.Handle(new DeleteTradingPlanCommand{TradingPlanId = tradingPlanId}, CancellationToken.None);


        // Assert
        result.Should().BeOfType<DeleteTradingPlanResult>();
        result.IsSuccess.Should().BeTrue();
        await _repo.Received(1).GetByIdAsync(Arg.Any<TradingPlanId>(), Arg.Any<CancellationToken>());
        await _repo.Received(1).DeleteAsync(Arg.Any<TradingPlan>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteHandle_ThrowInternalServerException_WhenStatusFalse()
    {
        // Arrange
        var tradingPlanId = Guid.NewGuid();
        var tradingPlan = TradingPlan.Create(TradingPlanId.Of(tradingPlanId), "Test", null, null, [],_userId);
        
        _repo.GetByIdAsync(tradingPlan.Id, Arg.Any<CancellationToken>())
            .Returns(tradingPlan);
        _repo.DeleteAsync(Arg.Any<TradingPlan>(), Arg.Any<CancellationToken>()).Returns(false);

        // Act
        var requestAction = async () =>
            await _sut.Handle(new DeleteTradingPlanCommand{TradingPlanId = tradingPlanId}, CancellationToken.None);

        // Assert
        await requestAction.Should()
            .ThrowAsync<InternalServerException>();
    }

    [Fact]
    public async Task DeleteHandle_ThrowNotFoundException_WhenTradingPlanIdIsInvalid()
    {
        // Arrange
        var tradingPlanId = Guid.NewGuid();
        
        _repo.GetByIdAsync(TradingPlanId.Of(tradingPlanId), Arg.Any<CancellationToken>())
            .Returns((TradingPlan?)null);
        
        _repo.DeleteAsync(Arg.Any<TradingPlan>(), Arg.Any<CancellationToken>()).Returns(true);

        // Act
        var requestAction = async () =>
            await _sut.Handle(new DeleteTradingPlanCommand{TradingPlanId = tradingPlanId}, CancellationToken.None);

        // Assert
        await requestAction.Should()
            .ThrowAsync<NotFoundException>();
    }
}