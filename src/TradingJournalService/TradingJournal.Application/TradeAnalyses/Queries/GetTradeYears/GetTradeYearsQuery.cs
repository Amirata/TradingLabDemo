namespace TradingJournal.Application.TradeAnalyses.Queries.GetTradeYears;

public class GetTradeYearsQuery
    : IQuery<ICollection<int>>
{
    public Guid PlanId { get; set; }
}

// public class GetTradeYearsResult
// {
//     public string Year { get; set; } = default!;
// }