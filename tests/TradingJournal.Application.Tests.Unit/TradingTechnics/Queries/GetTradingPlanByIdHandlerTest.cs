using BuildingBlocks.Exceptions;
using FluentAssertions;
using NSubstitute;
using TradingJournal.Application.Repositories;
using TradingJournal.Application.TradingTechnics.Queries.GetTradingTechnicById;

namespace TradingJournal.Application.Tests.Unit.TradingTechnics.Queries;

public class GetTradingTechnicByIdHandlerTest
{
    private readonly GetTradingTechnicByIdHandler _sut;
    private readonly ITradingTechnicRepository _repo = Substitute.For<ITradingTechnicRepository>();

    public GetTradingTechnicByIdHandlerTest()
    {
        _sut = new GetTradingTechnicByIdHandler(_repo);
    }

    [Fact]
    public async Task GetTradingTechnicByIdHandle_ReturnGetTradingTechnicByIdResult_WhenTradingTechnicIdValid()
    {
        // Arrange
        var Images = new List<string>()
        {
            "path1",
            "path2"
        };

        var getTradingTechnicByIdResult = new GetTradingTechnicByIdResult()
        {
            Id = Guid.Parse("3B0A0F86-E336-42E2-8FA8-A3388204B3C4"),
            Name = "Test Name",
            Description = "Test Description",
            Images = Images
        };

        _repo.GetTradingTechnicByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(getTradingTechnicByIdResult);

        // Act
        var result = await _sut.Handle(new GetTradingTechnicByIdQuery
        {
            TradingTechnicId = getTradingTechnicByIdResult.Id
        }, CancellationToken.None);


        // Assert
        result.Should().BeOfType<GetTradingTechnicByIdResult>();
        result.Should().BeEquivalentTo(getTradingTechnicByIdResult);
        await _repo.Received(1)
            .GetTradingTechnicByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetTradingTechnicByIdHandle_ThrowNotFoundException_WhenTradingTechnicIdIsInValid()
    {
        // Arrange
        _repo.GetTradingTechnicByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((GetTradingTechnicByIdResult?)null);

        // Act
        var requestAction = async () =>
            await _sut.Handle(new GetTradingTechnicByIdQuery
            {
                TradingTechnicId = Guid.NewGuid()
            }, CancellationToken.None);


        // Assert
        await requestAction.Should()
            .ThrowAsync<NotFoundException>();
    }
}