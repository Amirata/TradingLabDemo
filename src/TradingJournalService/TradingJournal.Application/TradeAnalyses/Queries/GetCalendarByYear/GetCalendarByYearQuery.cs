using BuildingBlocks.Pagination;
using TradingJournal.Domain.Enums;

namespace TradingJournal.Application.TradeAnalyses.Queries.GetCalendarByYear;

public class GetCalendarByYearQuery
    : IQuery<GetCalendarByYearResult>
{
    public Guid PlanId { get; set; }
    public int Year { get; set; }
    
}

public class GetCalendarByYearResult
{
    public List<TradeCalendar> Calendar { get; set; } = default!;
    public double RiskToRewardMean { get; set; }
    public double WinRate { get; set; }
    public double TotalTradeCount { get; set; }
    public int TotalWinTradeCount { get; set; }
    public int TotalLossTradeCount { get; set; }
    public double NetProfit { get; set; }
    public double GrossProfit { get; set; }
}

public class TradeCalendar
{
    public DateOnly Date { get; set; }

    public int Count { get; } = 1;
    
    /// <summary>
    /// 0 Loss
    /// 1 NoTrade
    /// 2 Profit
    /// </summary>
    public int Level { get; set; } 
}