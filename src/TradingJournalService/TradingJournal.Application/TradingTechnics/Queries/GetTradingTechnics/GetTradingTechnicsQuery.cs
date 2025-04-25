using BuildingBlocks.Pagination;

namespace TradingJournal.Application.TradingTechnics.Queries.GetTradingTechnics;

public record GetTradingTechnicsQuery(PaginationRequest PaginationRequest)
    : IQuery<PaginatedResult<GetTradingTechnicsResult>>;

public class GetTradingTechnicsResult
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public IEnumerable<string> Images { get; set; } = default!;
}