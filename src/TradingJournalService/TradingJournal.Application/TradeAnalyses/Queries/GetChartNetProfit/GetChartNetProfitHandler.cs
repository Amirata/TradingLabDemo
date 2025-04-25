using TradingJournal.Application.Repositories;

namespace TradingJournal.Application.TradeAnalyses.Queries.GetChartNetProfit;

public class GetChartNetProfitHandler(ITradeAnalyseRepository repo)
    : IQueryHandler<GetChartNetProfitQuery, ICollection<GetChartNetProfitResult>>
{
    public async Task<ICollection<GetChartNetProfitResult>> Handle(GetChartNetProfitQuery query, CancellationToken cancellationToken)
    {
        var getChartNetProfitResult = await repo.GetChartNetProfitAsync(query.PlanId, cancellationToken);

        return getChartNetProfitResult;
    }
}