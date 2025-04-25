using BuildingBlocks.Pagination;
using TradingJournal.Application.Repositories;

namespace TradingJournal.Application.TradingTechnics.Queries.GetTradingTechnics;

public class GetTradingTechnicsHandler(ITradingTechnicRepository repo)
    : IQueryHandler<GetTradingTechnicsQuery, PaginatedResult<GetTradingTechnicsResult>>
{
    public async Task<PaginatedResult<GetTradingTechnicsResult>> Handle(GetTradingTechnicsQuery query, CancellationToken cancellationToken)
    {
        var getTradingTechnicsResult = await repo.GetTradingTechnicsAsync(query, cancellationToken);

        return getTradingTechnicsResult;
    }
}