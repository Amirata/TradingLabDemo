using TradingJournal.Domain.Enums;

namespace TradingJournal.Application.TradeAnalyses.Queries.GetGrossAndNetForEachSymbol;

public class GetGrossAndNetForEachSymbolQuery
    : IQuery<ICollection<GetGrossAndNetForEachSymbolResult>>
{
    public Guid PlanId { get; set; }
    public DateOnly? FromDate { get; set; }
    public DateOnly? ToDate { get; set; }
}

public class GetGrossAndNetForEachSymbolResult
{
    public Symbols Symbol { get; set; }
    public double NetProfit { get; set; }
    public double GrossProfit { get; set; }
}
