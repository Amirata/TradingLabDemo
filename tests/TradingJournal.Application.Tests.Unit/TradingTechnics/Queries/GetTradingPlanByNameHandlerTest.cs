using FluentAssertions;
using NSubstitute;
using TradingJournal.Application.Repositories;
using TradingJournal.Application.TradingTechnics.Queries.GetTradingTechnicByName;

namespace TradingJournal.Application.Tests.Unit.TradingTechnics.Queries;

public class GetTradingTechnicByNameHandlerTest
{
    private readonly GetTradingTechnicByNameHandler _sut;
    private readonly ITradingTechnicRepository _repo = Substitute.For<ITradingTechnicRepository>();

    public GetTradingTechnicByNameHandlerTest()
    {
        _sut = new GetTradingTechnicByNameHandler(_repo);
    }

    [Fact]
    public async Task GetTradingTechnicByNameHandle_ReturnGetTradingTechnicByNameResult_WhenTradingTechnicNameValid()
    {
        // Arrange
        var Images = new List<string>()
        {
            "path1",
            "path2"
        };

        var getTradingTechnicByNameResult = new GetTradingTechnicByNameResult()
        {
            Id = Guid.Parse("3B0A0F86-E336-42E2-8FA8-A3388204B3C4"),
            Name = "Test Name",
            Description = "Test Description",
            Images = Images
        };

        _repo.GetTradingTechnicByNameAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns([getTradingTechnicByNameResult]);

        // Act
        var result = await _sut.Handle(new GetTradingTechnicByNameQuery
            {
                Name = getTradingTechnicByNameResult.Name
            },
            CancellationToken.None);


        // Assert
        result.Should().BeOfType<List<GetTradingTechnicByNameResult>>();
        result.Should().BeEquivalentTo([getTradingTechnicByNameResult]);
        await _repo.Received(1)
            .GetTradingTechnicByNameAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetTradingTechnicByNameHandle_ReturnEmptyResult_WhenTradingTechnicNameIsInValid()
    {
        // Arrange
        _repo.GetTradingTechnicByNameAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns([]);

        // Act
        var result = await _sut.Handle(new GetTradingTechnicByNameQuery
            {
                Name = "Test Name"
            },
            CancellationToken.None);


        // Assert
        result.Should().BeOfType<List<GetTradingTechnicByNameResult>>();
        result.Should().BeEquivalentTo(new List<GetTradingTechnicByNameResult>());
        await _repo.Received(1)
            .GetTradingTechnicByNameAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());

    }
}