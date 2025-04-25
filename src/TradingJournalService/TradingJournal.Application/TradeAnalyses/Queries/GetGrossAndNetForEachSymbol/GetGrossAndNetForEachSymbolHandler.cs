using TradingJournal.Application.Repositories;

namespace TradingJournal.Application.TradeAnalyses.Queries.GetGrossAndNetForEachSymbol;

public class GetGrossAndNetForEachSymbolHandler(ITradeAnalyseRepository repo)
    : IQueryHandler<GetGrossAndNetForEachSymbolQuery, ICollection<GetGrossAndNetForEachSymbolResult>>
{
    public async Task<ICollection<GetGrossAndNetForEachSymbolResult>> Handle(GetGrossAndNetForEachSymbolQuery query, CancellationToken cancellationToken)
    {
        var getGrossAndNetForEachSymbolResult = await repo.GetGrossAndNetForEachSymbolAsync(query.PlanId, query.FromDate, query.ToDate, cancellationToken);

        return getGrossAndNetForEachSymbolResult;
    }
}