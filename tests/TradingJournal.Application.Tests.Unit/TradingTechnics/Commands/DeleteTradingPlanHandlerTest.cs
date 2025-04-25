using BuildingBlocks.Exceptions;
using FluentAssertions;
using NSubstitute;
using TradingJournal.Application.Repositories;
using TradingJournal.Application.TradingTechnics.Commands.DeleteTradingTechnic;
using TradingJournal.Domain.Models;
using TradingJournal.Domain.ValueObjects;

namespace TradingJournal.Application.Tests.Unit.TradingTechnics.Commands;

public class DeleteTradingTechnicHandlerTest
{
    private readonly DeleteTradingTechnicHandler _sut;
    private readonly ITradingTechnicRepository _repo = Substitute.For<ITradingTechnicRepository>();
    private readonly UserId _userId;
    public DeleteTradingTechnicHandlerTest()
    {
        _sut = new DeleteTradingTechnicHandler(_repo);
        _userId = UserId.Of(Guid.Parse("621CEFBD-4020-4AC1-AC02-B5BB9DDBF1B6"));
    }

    [Fact]
    public async Task DeleteHandle_ReturnTure_WhenSuccessfullyDelete()
    {
        // Arrange
        var tradingTechnicId = Guid.NewGuid();
        var tradingTechnic = TradingTechnic.Create(TradingTechnicId.Of(tradingTechnicId), "name", "description",_userId);
        
        _repo.GetByIdAsync(tradingTechnic.Id, Arg.Any<CancellationToken>())
            .Returns(tradingTechnic);
        _repo.DeleteAsync(Arg.Any<TradingTechnic>(), Arg.Any<CancellationToken>()).Returns(true);

        // Act
        var result = await _sut.Handle(new DeleteTradingTechnicCommand
        {
            TradingTechnicId = tradingTechnicId
        }, CancellationToken.None);


        // Assert
        result.Should().BeOfType<DeleteTradingTechnicResult>();
        result.IsSuccess.Should().BeTrue();
        await _repo.Received(1).GetByIdAsync(Arg.Any<TradingTechnicId>(), Arg.Any<CancellationToken>());
        await _repo.Received(1).DeleteAsync(Arg.Any<TradingTechnic>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteHandle_ThrowInternalServerException_WhenStatusFalse()
    {
        // Arrange
        var tradingTechnicId = Guid.NewGuid();
        var tradingTechnic = TradingTechnic.Create(TradingTechnicId.Of(tradingTechnicId), "name", "description",_userId);
        
        _repo.GetByIdAsync(tradingTechnic.Id, Arg.Any<CancellationToken>())
            .Returns(tradingTechnic);
        _repo.DeleteAsync(Arg.Any<TradingTechnic>(), Arg.Any<CancellationToken>()).Returns(false);

        // Act
        var requestAction = async () =>
            await _sut.Handle(new DeleteTradingTechnicCommand
            {
                TradingTechnicId = tradingTechnicId
            }, CancellationToken.None);

        // Assert
        await requestAction.Should()
            .ThrowAsync<InternalServerException>();
    }

    [Fact]
    public async Task DeleteHandle_ThrowNotFoundException_WhenTradingTechnicIdIsInvalid()
    {
        // Arrange
        var tradingTechnicId = Guid.NewGuid();
        
        _repo.GetByIdAsync(TradingTechnicId.Of(tradingTechnicId), Arg.Any<CancellationToken>())
            .Returns((TradingTechnic?)null);
        
        _repo.DeleteAsync(Arg.Any<TradingTechnic>(), Arg.Any<CancellationToken>()).Returns(true);

        // Act
        var requestAction = async () =>
            await _sut.Handle(new DeleteTradingTechnicCommand
            {
                TradingTechnicId = tradingTechnicId
            }, CancellationToken.None);

        // Assert
        await requestAction.Should()
            .ThrowAsync<NotFoundException>();
    }
}