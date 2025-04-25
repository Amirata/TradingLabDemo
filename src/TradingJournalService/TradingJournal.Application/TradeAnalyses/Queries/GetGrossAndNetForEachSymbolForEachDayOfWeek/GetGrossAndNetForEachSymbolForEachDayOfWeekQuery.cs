using TradingJournal.Domain.Enums;

namespace TradingJournal.Application.TradeAnalyses.Queries.GetGrossAndNetForEachSymbolForEachDayOfWeek;

public class GetGrossAndNetForEachSymbolForEachDayOfWeekQuery
    : IQuery<ICollection<GetGrossAndNetForEachSymbolForEachDayOfWeekResult>>
{
    public Guid PlanId { get; set; }
    public DateOnly? FromDate { get; set; }
    public DateOnly? ToDate { get; set; }
    public Symbols? Symbol { get; set; } 
}

public class GetGrossAndNetForEachSymbolForEachDayOfWeekResult
{
    public int DayOfWeek { get; set; } 
    public double NetProfit { get; set; }
    public double GrossProfit { get; set; }
}

public class GetGrossAndNetForEachSymbolForEachDayOfWeekFromQuery
{
    public DateOnly? FromDate { get; set; }
    public DateOnly? ToDate { get; set; }
    public Symbols? Symbol { get; set; } 
}
