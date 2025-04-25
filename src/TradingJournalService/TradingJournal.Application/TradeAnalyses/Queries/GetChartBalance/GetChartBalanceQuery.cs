namespace TradingJournal.Application.TradeAnalyses.Queries.GetChartBalance;

public class GetChartBalanceQuery
    : IQuery<ICollection<GetChartBalanceResult>>
{
    public Guid PlanId { get; set; }
}

public class GetChartBalanceResult
{
    public DateTime DateTime { get; set; } = default!;
    public double Balance { get; set; }
}
