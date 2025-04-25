using TradingJournal.Application.TradeAnalyses.Queries.GetCalendarByYear;
using TradingJournal.Application.TradeAnalyses.Queries.GetChartBalance;
using TradingJournal.Application.TradeAnalyses.Queries.GetChartNetProfit;
using TradingJournal.Application.TradeAnalyses.Queries.GetGrossAndNetForEachSymbol;
using TradingJournal.Application.TradeAnalyses.Queries.GetGrossAndNetForEachSymbolForEachDayOfWeek;
using TradingJournal.Domain.Enums;

namespace TradingJournal.Application.Repositories;
public interface ITradeAnalyseRepository
{
   Task<ICollection<int>> GetTradeYearsAsync(Guid planId, CancellationToken cancellationToken);
   Task<GetCalendarByYearResult> GetCalendarByYearAsync(Guid planId,int year, CancellationToken cancellationToken);
   Task<ICollection<GetChartNetProfitResult>> GetChartNetProfitAsync(Guid planId, CancellationToken cancellationToken);
   Task<ICollection<GetChartBalanceResult>> GetChartBalanceAsync(Guid planId, CancellationToken cancellationToken);
   Task<ICollection<GetGrossAndNetForEachSymbolResult>> GetGrossAndNetForEachSymbolAsync(Guid planId,DateOnly? fromDate, DateOnly? toDate, CancellationToken cancellationToken);
   Task<ICollection<GetGrossAndNetForEachSymbolForEachDayOfWeekResult>> GetGrossAndNetForEachSymbolForEachDayOfWeekAsync(Guid planId ,Symbols? symbol ,DateOnly? fromDate, DateOnly? toDate, CancellationToken cancellationToken);
}
