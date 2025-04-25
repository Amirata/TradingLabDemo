using BuildingBlocks.Exceptions;
using FluentAssertions;
using NSubstitute;
using TradingJournal.Application.Repositories;
using TradingJournal.Application.TradingTechnics.Commands.CreateTradingTechnic;
using TradingJournal.Domain.Models;
using TradingJournal.Domain.ValueObjects;

namespace TradingJournal.Application.Tests.Unit.TradingTechnics.Commands;

public class CreateTradingTechnicHandlerTest
{
    private readonly CreateTradingTechnicHandler _sut;
    private readonly ITradingTechnicRepository _repo = Substitute.For<ITradingTechnicRepository>();
    private readonly Guid _userId;

    public CreateTradingTechnicHandlerTest()
    {
        _sut = new CreateTradingTechnicHandler(_repo);
        _userId = Guid.Parse("621CEFBD-4020-4AC1-AC02-B5BB9DDBF1B6");
    }

    [Fact]
    public async Task CreateHandle_ReturnTradingTechnicId_WhenTradingTechnicDtoValid()
    {
        // Arrange
        var pictures = new List<string>()
        {
            "path1",
            "path2"
        };

        var createTradingTechnicCommand = new CreateTradingTechnicCommand()
        {
            UserId = _userId,
            Name = "Test Name",
            Description = "Test Description",
            NewImages = []
        };

        _repo.CreateAsync(Arg.Any<TradingTechnic>(), Arg.Any<CancellationToken>()).Returns(true);

        // Act
        var result = await _sut.Handle(createTradingTechnicCommand,
            CancellationToken.None);


        // Assert
        result.Should().BeOfType<CreateTradingTechnicResult>();
        result.Id.Should().NotBeEmpty();
        await _repo.Received(1).CreateAsync(Arg.Any<TradingTechnic>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateHandle_ThrowInternalServerException_WhenStatusFalse()
    {
        // Arrange
        var pictures = new List<string>()
        {
             "path1",
             "path2"
        };

        var createTradingTechnicCommand = new CreateTradingTechnicCommand()
        {
            UserId = _userId,
            Name = "Test Name",
            Description = "Test Description",
            NewImages = []
        };

        _repo.CreateAsync(Arg.Any<TradingTechnic>(), Arg.Any<CancellationToken>()).Returns(false);

        // Act
        var requestAction = async () =>
            await _sut.Handle(createTradingTechnicCommand,
                CancellationToken.None);
        // Assert
        await requestAction.Should()
            .ThrowAsync<InternalServerException>();
    }
}