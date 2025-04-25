using BuildingBlocks.Pagination;
using TradingJournal.Application.Repositories;

namespace TradingJournal.Application.TradingPlans.Queries.GetTradingPlans;

public class GetTradingPlansHandler(ITradingPlanRepository repo)
    : IQueryHandler<GetTradingPlansQuery, PaginatedResult<GetTradingPlansResult>>
{
    public async Task<PaginatedResult<GetTradingPlansResult>> Handle(GetTradingPlansQuery query, CancellationToken cancellationToken)
    {
        
        var getTradingPlansResult = await repo.GetTradingPlansAsync(query, cancellationToken);

        return getTradingPlansResult;
    }
}