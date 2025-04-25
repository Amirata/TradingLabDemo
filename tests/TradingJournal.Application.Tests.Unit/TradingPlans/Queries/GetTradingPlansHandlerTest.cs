using BuildingBlocks.Pagination;
using FluentAssertions;
using NSubstitute;
using TradingJournal.Application.Repositories;
using TradingJournal.Application.TradingPlans.Queries.GetTradingPlans;
using TradingJournal.Application.TradingTechnics.Queries.GetTradingTechnicById;
using TradingJournal.Domain.ValueObjects;

namespace TradingJournal.Application.Tests.Unit.TradingPlans.Queries;

public class GetTradingPlansHandlerTest
{
    private readonly GetTradingPlansHandler _sut;
    private readonly ITradingPlanRepository _repo = Substitute.For<ITradingPlanRepository>();

    public GetTradingPlansHandlerTest()
    {
        _sut = new GetTradingPlansHandler(_repo);
    }

    [Fact]
    public async Task GetTradingPlansHandle_ReturnPaginatedResultOfGetTradingPlansResult_WhenTradingPlansExists()
    {
        // Arrange
        var getTradingPlansResultList = new List<GetTradingPlansResult>();

        for (var i = 1; i <= 6; i++)
        {
            var getTradingPlansResult = new GetTradingPlansResult()
            {
                Id = Guid.NewGuid(),
                Name = $"Test Name {i}",
                Technics = new List<GetTradingTechnicByIdResult>
                {
                    new GetTradingTechnicByIdResult()
                    {
                        Id = Guid.NewGuid(),
                        Name = $"Technic {i}",
                        Description = $"Technic {i} description",
                    },
                },
                FromTime = new TimeOnly(00, 00, 00),
                ToTime = new TimeOnly(21, 30, 00),
                SelectedDays = new List<string>() { "Monday", "Tuesday", "Wednesday", "Thursday" }
            };
            getTradingPlansResultList.Add(getTradingPlansResult);
        }


        _repo.CountAsync(Arg.Any<string?>(), Arg.Any<CancellationToken>())
            .Returns(6);
        _repo.GetTradingPlansAsync(Arg.Any<GetTradingPlansQuery>(), Arg.Any<CancellationToken>())
            .Returns(PaginatedResult<GetTradingPlansResult>.Create(
                getTradingPlansResultList.Take(5).ToList(),
                6,
                1,
                5
            ));

        // Act
        var result = await _sut.Handle(new GetTradingPlansQuery
        {
            PaginationRequest = new PaginationRequest
            {
                PageNumber = 1,
                PageSize = 5
            }
        }, CancellationToken.None);


        // Assert
        result.Should().BeOfType<PaginatedResult<GetTradingPlansResult>>();
        result.Data.Should().BeEquivalentTo(getTradingPlansResultList.Take(5).ToList());
        result.TotalCount.Should().Be(6);
        result.CurrentPage.Should().Be(1);
        result.PageSize.Should().Be(5);
        result.TotalPages.Should().Be(2);
       
        await _repo.Received(1)
            .GetTradingPlansAsync(Arg.Any<GetTradingPlansQuery>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetTradingPlansHandle_ReturnPaginatedResultOfGetTradingPlansResult_WhenTradingPlansNotExists()
    {
        // Arrange
        var getTradingPlansResultList = new List<GetTradingPlansResult>();


        _repo.CountAsync(Arg.Any<string?>(), Arg.Any<CancellationToken>())
            .Returns(0);

        _repo.GetTradingPlansAsync(Arg.Any<GetTradingPlansQuery>(), Arg.Any<CancellationToken>())
            .Returns(PaginatedResult<GetTradingPlansResult>.Create(
                getTradingPlansResultList,
                0,
                1,
                5
            ));

        // Act
        var result = await _sut.Handle(new GetTradingPlansQuery
        {
            PaginationRequest = new PaginationRequest
            {
                PageNumber = 1,
                PageSize = 5
            }
        }, CancellationToken.None);


        // Assert
        result.Should().BeOfType<PaginatedResult<GetTradingPlansResult>>();
        result.Data.Should().BeEquivalentTo(getTradingPlansResultList);
        result.TotalCount.Should().Be(0);
        result.CurrentPage.Should().Be(1);
        result.PageSize.Should().Be(5);
        result.TotalPages.Should().Be(0);
     

        await _repo.Received(1)
            .GetTradingPlansAsync(Arg.Any<GetTradingPlansQuery>(), Arg.Any<CancellationToken>());
    }
}