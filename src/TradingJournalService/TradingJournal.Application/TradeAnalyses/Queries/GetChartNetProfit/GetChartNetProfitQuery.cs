namespace TradingJournal.Application.TradeAnalyses.Queries.GetChartNetProfit;

public class GetChartNetProfitQuery
    : IQuery<ICollection<GetChartNetProfitResult>>
{
    public Guid PlanId { get; set; }
}

public class GetChartNetProfitResult
{
    public string Date { get; set; } = default!;
    public double NetProfit { get; set; }
}
