using BuildingBlocks.Exceptions;
using FluentAssertions;
using NSubstitute;
using TradingJournal.Application.Repositories;
using TradingJournal.Application.TradingTechnics.Commands.UpdateTradingTechnic;
using TradingJournal.Domain.Models;
using TradingJournal.Domain.ValueObjects;

namespace TradingJournal.Application.Tests.Unit.TradingTechnics.Commands;

public class UpdateTradingTechnicHandlerTest
{
    private readonly UpdateTradingTechnicHandler _sut;
    private readonly ITradingTechnicRepository _repo = Substitute.For<ITradingTechnicRepository>();
    private readonly UserId _userId;
    public UpdateTradingTechnicHandlerTest()
    {
        _sut = new UpdateTradingTechnicHandler(_repo);
        _userId = UserId.Of(Guid.Parse("621CEFBD-4020-4AC1-AC02-B5BB9DDBF1B6"));
    }

    [Fact]
    public async Task UpdateHandle_ReturnTrue_WhenTradingTechnicValid()
    {
        // Arrange
       

        var updateTradingTechnicCommand = new UpdateTradingTechnicCommand()
        {
            Id = Guid.Parse("3B0A0F86-E336-42E2-8FA8-A3388204B3C7"),
            Name = "Test Name",
            Description = "Test Description",
            Images = [],
            RemovedImages = [],
            NewImages = []
            
        };


        var tradingTechnic = TradingTechnic.Create(
            TradingTechnicId.Of(updateTradingTechnicCommand.Id),
            updateTradingTechnicCommand.Name,
            updateTradingTechnicCommand.Description,
            _userId
        );

      

        _repo.GetByIdAsync(TradingTechnicId.Of(updateTradingTechnicCommand.Id), Arg.Any<CancellationToken>())
            .Returns(tradingTechnic);

        _repo.UpdateAsync(Arg.Any<TradingTechnic>(), Arg.Any<CancellationToken>()).Returns(true);

        // Act
        var result = await _sut.Handle(updateTradingTechnicCommand,
            CancellationToken.None);


        // Assert
        result.Should().BeOfType<UpdateTradingTechnicResult>();
        result.IsSuccess.Should().BeTrue();
        await _repo.Received(1).UpdateAsync(Arg.Any<TradingTechnic>(), Arg.Any<CancellationToken>());
        await _repo.Received(1).GetByIdAsync(Arg.Any<TradingTechnicId>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateHandle_ThrowInternalServerException_WhenStatusFalse()
    {
        // Arrange

        var updateTradingTechnicCommand = new UpdateTradingTechnicCommand()
        {
            Id = Guid.Parse("3B0A0F86-E336-42E2-8FA8-A3388204B3C7"),
            Name = "Test Name",
            Description = "Test Description",
            Images = [],
            RemovedImages = [],
            NewImages = []
        };


        var tradingTechnic = TradingTechnic.Create(
            TradingTechnicId.Of(updateTradingTechnicCommand.Id),
            updateTradingTechnicCommand.Name,
            updateTradingTechnicCommand.Description,
            _userId
        );

        

        _repo.GetByIdAsync(TradingTechnicId.Of(updateTradingTechnicCommand.Id), Arg.Any<CancellationToken>())
            .Returns(tradingTechnic);

        _repo.UpdateAsync(Arg.Any<TradingTechnic>(), Arg.Any<CancellationToken>()).Returns(false);

        // Act
        var requestAction = async () =>
            await _sut.Handle(updateTradingTechnicCommand, CancellationToken.None);

        // Assert
        await requestAction.Should()
            .ThrowAsync<InternalServerException>();
    }

    [Fact]
    public async Task UpdateHandle_ThrowNotFoundException_WhenTradingTechnicIdIsInvalid()
    {
        // Arrange
       

        var updateTradingTechnicCommand = new UpdateTradingTechnicCommand()
        {
            Id = Guid.Parse("3B0A0F86-E336-42E2-8FA8-A3388204B3C7"),
            Name = "Test Name",
            Description = "Test Description",
            Images = [],
            RemovedImages = [],
            NewImages = []
        };


        _repo.GetByIdAsync(TradingTechnicId.Of(updateTradingTechnicCommand.Id), Arg.Any<CancellationToken>())
            .Returns((TradingTechnic?)null);

        _repo.UpdateAsync(Arg.Any<TradingTechnic>(), Arg.Any<CancellationToken>()).Returns(true);

        // Act
        var requestAction = async () =>
            await _sut.Handle(updateTradingTechnicCommand, CancellationToken.None);

        // Assert
        await requestAction.Should()
            .ThrowAsync<NotFoundException>();
    }
}