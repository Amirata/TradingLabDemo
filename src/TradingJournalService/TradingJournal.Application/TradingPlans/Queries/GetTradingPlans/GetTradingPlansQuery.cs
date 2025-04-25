using BuildingBlocks.Pagination;
using TradingJournal.Application.TradingTechnics.Queries.GetTradingTechnicById;

namespace TradingJournal.Application.TradingPlans.Queries.GetTradingPlans;

public class GetTradingPlansQuery
    : IQuery<PaginatedResult<GetTradingPlansResult>>
{
    public PaginationRequest PaginationRequest { get; set; } = default!;
}

public class GetTradingPlansResult
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public TimeOnly? FromTime { get; set; }
    public TimeOnly? ToTime { get; set; }
    public required ICollection<string> SelectedDays { get; set; }
    public required ICollection<GetTradingTechnicByIdResult> Technics { get; set; }
}