using BuildingBlocks.Pagination;
using TradingJournal.Application.Repositories;

namespace TradingJournal.Application.TradeAnalyses.Queries.GetCalendarByYear;

public class GetCalendarByYearHandler(ITradeAnalyseRepository repo)
    : IQueryHandler<GetCalendarByYearQuery, GetCalendarByYearResult>
{
    public async Task<GetCalendarByYearResult> Handle(GetCalendarByYearQuery query, CancellationToken cancellationToken)
    {
        var getCalendarByYearResult = await repo.GetCalendarByYearAsync(query.PlanId, query.Year, cancellationToken);

        return getCalendarByYearResult;
    }
}