using BuildingBlocks.Pagination;
using FluentAssertions;
using NSubstitute;
using TradingJournal.Application.Repositories;
using TradingJournal.Application.TradingTechnics.Queries.GetTradingTechnics;

namespace TradingJournal.Application.Tests.Unit.TradingTechnics.Queries;

public class GetTradingTechnicsHandlerTest
{
    private readonly GetTradingTechnicsHandler _sut;
    private readonly ITradingTechnicRepository _repo = Substitute.For<ITradingTechnicRepository>();

    public GetTradingTechnicsHandlerTest()
    {
        _sut = new GetTradingTechnicsHandler(_repo);
    }

    [Fact]
    public async Task
        GetTradingTechnicsHandle_ReturnPaginatedResultOfGetTradingTechnicsResult_WhenTradingTechnicsExists()
    {
        // Arrange
        var getTradingTechnicsResultList = new List<GetTradingTechnicsResult>();

        for (var i = 1; i <= 6; i++)
        {
            var getTradingTechnicsResult = new GetTradingTechnicsResult()
            {
                Id = Guid.NewGuid(),
                Name = $"Test Name {i}",
                Description = $"Test Description {i}",
                Images =
                [
                    $"path {i}"
                ]
            };
            getTradingTechnicsResultList.Add(getTradingTechnicsResult);
        }
        

        _repo.GetTradingTechnicsAsync(Arg.Any<GetTradingTechnicsQuery>(), Arg.Any<CancellationToken>())
            .Returns(PaginatedResult<GetTradingTechnicsResult>.Create(
                getTradingTechnicsResultList.Take(5).ToList(),
                6,
                1,
                5
            ));

        // Act
        var result = await _sut.Handle(new GetTradingTechnicsQuery(new PaginationRequest
            {
                PageNumber = 1,
                PageSize = 5
            }),
            CancellationToken.None);


        // Assert
        result.Should().BeOfType<PaginatedResult<GetTradingTechnicsResult>>();
        result.Data.Should().BeEquivalentTo(getTradingTechnicsResultList.Take(5).ToList());
        result.TotalCount.Should().Be(6);
        result.CurrentPage.Should().Be(1);
        result.PageSize.Should().Be(5);
        result.TotalPages.Should().Be(2);
       
        await _repo.Received(1)
            .GetTradingTechnicsAsync(Arg.Any<GetTradingTechnicsQuery>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task
        GetTradingTechnicsHandle_ReturnPaginatedResultGetTradingTechnicsResult_WhenTradingTechnicsNotExists()
    {
        // Arrange
        var getTradingTechnicsResultList = new List<GetTradingTechnicsResult>();

        
        var getTradingTechnicsQuery = new GetTradingTechnicsQuery
        (
            new PaginationRequest()
            {
                PageNumber = 1,
                PageSize = 5
            }
        );
        
        _repo.GetTradingTechnicsAsync(Arg.Any<GetTradingTechnicsQuery>(), Arg.Any<CancellationToken>())
            .Returns(PaginatedResult<GetTradingTechnicsResult>.Create(
                getTradingTechnicsResultList,
                0,
                1,
                5
            ));

        // Act
        var result = await _sut.Handle(new GetTradingTechnicsQuery(new PaginationRequest
            {
                PageNumber = 1,
                PageSize = 5
            }),
            CancellationToken.None);


        // Assert
        result.Should().BeOfType<PaginatedResult<GetTradingTechnicsResult>>();
        result.Data.Should().BeEquivalentTo(getTradingTechnicsResultList);
        result.TotalCount.Should().Be(0);
        result.CurrentPage.Should().Be(1);
        result.PageSize.Should().Be(5);
        result.TotalPages.Should().Be(0);
       
        await _repo.Received(1)
            .GetTradingTechnicsAsync(Arg.Any<GetTradingTechnicsQuery>(), Arg.Any<CancellationToken>());
    }
}