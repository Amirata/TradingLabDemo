using BuildingBlocks.Pagination;
using TradingJournal.Application.Repositories;

namespace TradingJournal.Application.Trades.Queries.GetTrades;

public class GetTradesHandler(ITradeRepository repo)
    : IQueryHandler<GetTradesQuery, PaginatedResult<GetTradesResult>>
{
    public async Task<PaginatedResult<GetTradesResult>> Handle(GetTradesQuery query, CancellationToken cancellationToken)
    {
        //var pageIndex = query.PaginationRequestWithId.PageIndex;
        //var pageSize = query.PaginationRequestWithId.PageSize;

        //var totalCount = await repo.CountAsync(query.PaginationRequestWithId.Search, query.PaginationRequestWithId.Id, cancellationToken);

        var getTradesResult = await repo.GetTradesAsync(query, cancellationToken);

        return 
            getTradesResult;
    }
}