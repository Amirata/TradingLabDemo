using TradingJournal.Application.Repositories;

namespace TradingJournal.Application.TradeAnalyses.Queries.GetGrossAndNetForEachSymbolForEachDayOfWeek;

public class GetGrossAndNetForEachSymbolsForEachDaysOfWeekHandler(ITradeAnalyseRepository repo)
    : IQueryHandler<GetGrossAndNetForEachSymbolForEachDayOfWeekQuery, ICollection<GetGrossAndNetForEachSymbolForEachDayOfWeekResult>>
{
    public async Task<ICollection<GetGrossAndNetForEachSymbolForEachDayOfWeekResult>> Handle(GetGrossAndNetForEachSymbolForEachDayOfWeekQuery query, CancellationToken cancellationToken)
    {
        var getGrossAndNetForEachSymbolForEachDayOfWeekResult = await repo.GetGrossAndNetForEachSymbolForEachDayOfWeekAsync(query.PlanId, query.Symbol, query.FromDate, query.ToDate, cancellationToken);

        return getGrossAndNetForEachSymbolForEachDayOfWeekResult;
    }
}