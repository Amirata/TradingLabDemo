using TradingJournal.Application.Repositories;

namespace TradingJournal.Application.TradeAnalyses.Queries.GetChartBalance;

public class GetChartBalanceHandler(ITradeAnalyseRepository repo)
    : IQueryHandler<GetChartBalanceQuery, ICollection<GetChartBalanceResult>>
{
    public async Task<ICollection<GetChartBalanceResult>> Handle(GetChartBalanceQuery query, CancellationToken cancellationToken)
    {
        var getChartBalanceResult = await repo.GetChartBalanceAsync(query.PlanId, cancellationToken);

        return getChartBalanceResult;
    }
}