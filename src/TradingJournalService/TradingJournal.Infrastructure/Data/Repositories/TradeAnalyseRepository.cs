using BuildingBlocks.Services;
using TradingJournal.Application.TradeAnalyses.Queries.GetCalendarByYear;
using TradingJournal.Application.TradeAnalyses.Queries.GetChartBalance;
using TradingJournal.Application.TradeAnalyses.Queries.GetChartNetProfit;
using TradingJournal.Application.TradeAnalyses.Queries.GetGrossAndNetForEachSymbol;
using TradingJournal.Application.TradeAnalyses.Queries.GetGrossAndNetForEachSymbolForEachDayOfWeek;
using TradingJournal.Domain.Enums;
using DateOnly = System.DateOnly;

namespace TradingJournal.Infrastructure.Data.Repositories;

public class TradeAnalyseRepository(ApplicationDbContext context, ICurrentSessionProvider currentSessionProvider) : ITradeAnalyseRepository
{
    private readonly Guid? _userId = currentSessionProvider.GetUserId();
    
    public async Task<ICollection<int>> GetTradeYearsAsync(Guid planId, CancellationToken cancellationToken)
    {
        if (_userId == default)
        {
            throw new UnauthorizedAccessException();
        }
        
        return await context.Trades
            .AsNoTracking()
            .Where(t=>t.TradingPlanId == TradingPlanId.Of(planId) && _userId == t.CreatedBy)
            .Select(t => t.EntryDateTime.Year) 
            .Distinct() 
            .OrderByDescending(year => year) 
            .ToListAsync(cancellationToken); 
    }

    public async Task<GetCalendarByYearResult> GetCalendarByYearAsync(Guid planId, int year,
        CancellationToken cancellationToken)
    {
        if (_userId == default)
        {
            throw new UnauthorizedAccessException();
        }
        
        var tradingPlanId = TradingPlanId.Of(planId);
        var query = context.Trades.AsNoTracking()
            .Where(t => t.TradingPlanId == tradingPlanId && t.EntryDateTime.Year == year && _userId == t.CreatedBy)
            .GroupBy(t => 1); // Group all rows together (single group);

        var data =  await query.Select(g => new GetCalendarByYearResult
        {
            GrossProfit = Math.Round(g.Sum(t => t.GrossProfit),2),
            NetProfit = Math.Round(g.Sum(t => t.NetProfit),2),
            TotalLossTradeCount = g.Count(t => t.NetProfit < 0),
            TotalWinTradeCount = g.Count(t => t.NetProfit > 0),
            TotalTradeCount = g.Count(),
            WinRate = g.Count() != 0 ? Math.Round(g.Count(t => t.NetProfit > 0) / (double)g.Count() * 100,2): 0,
            RiskToRewardMean = Math.Round(g
                .Average(t => t.EntryPrice - t.StopLossPrice != 0 ? Math.Abs(t.ClosePrice - t.EntryPrice) / Math.Abs(t.EntryPrice - t.StopLossPrice):0),2),
            Calendar = g.GroupBy(t=>DateOnly.FromDateTime(t.EntryDateTime)).Select(gr => new TradeCalendar
            {
                Date = gr.Key,
                Level = Math.Round(gr.Sum(t => t.NetProfit),2) > 0 ? 2 : Math.Round(gr.Sum(t => t.NetProfit),2) < 0 ? 0 : 1
            }).ToList()
        }).FirstOrDefaultAsync(cancellationToken);

        if (data == null)
        {
            return new GetCalendarByYearResult();
        }

        if (!data.Calendar.Select(t => t.Date).Contains(new DateOnly(year, 01, 01)))
        {
            data.Calendar.Insert(0, new TradeCalendar
            {
                Date = new DateOnly(year, 01, 01),
                Level = 1,
            });
        }
        
        if (!data.Calendar.Select(t => t.Date).Contains(new DateOnly(year, 12, 31)))
        {
            data.Calendar.Add(new TradeCalendar
            {
                Date = new DateOnly(year, 12, 31),
                Level = 1,
            });
        }
        return data;
    }

    public async Task<ICollection<GetChartNetProfitResult>> GetChartNetProfitAsync(
        Guid planId,
        CancellationToken cancellationToken)
    {
        if (_userId == default)
        {
            throw new UnauthorizedAccessException("User authentication required.");
        }

        var tradingPlanId = TradingPlanId.Of(planId);

        return await context.Trades
            .AsNoTracking()
            .Where(t => t.TradingPlanId == tradingPlanId && t.CreatedBy == _userId)
            .GroupBy(t => DateOnly.FromDateTime(t.EntryDateTime))
            .OrderBy(g => g.Key) 
            .Select(g => new GetChartNetProfitResult
            {
                Date = g.Key.ToString("yyyy-MM-dd"),
                NetProfit = Math.Round(g.Sum(t => t.NetProfit), 2)
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<ICollection<GetChartBalanceResult>> GetChartBalanceAsync(Guid planId, CancellationToken cancellationToken)
    {
        if (_userId == default)
        {
            throw new UnauthorizedAccessException();
        }

        var tradesQuery = context.Trades.AsNoTracking()
            .Where(t => t.TradingPlanId == TradingPlanId.Of(planId) && _userId == t.CreatedBy)
            .OrderBy(t => t.CloseDateTime);

        var firstTrade = await tradesQuery
            .Select(t => new { t.EntryDateTime ,t.CloseDateTime, t.Balance, t.NetProfit })
            .FirstOrDefaultAsync(cancellationToken);

        if (firstTrade == null)
        {
            return [];
        }

        var results = await tradesQuery
            .Select(t => new GetChartBalanceResult
            {
                DateTime = t.CloseDateTime,
                Balance = t.Balance
            })
            .ToListAsync(cancellationToken);

        results.Insert(0, new GetChartBalanceResult
        {
            DateTime = firstTrade.EntryDateTime.Date.AddDays(-1),
            Balance = firstTrade.Balance - firstTrade.NetProfit
        });

        return results;
    }

    public async Task<ICollection<GetGrossAndNetForEachSymbolResult>> GetGrossAndNetForEachSymbolAsync(Guid planId, DateOnly? fromDate, DateOnly? toDate,
        CancellationToken cancellationToken)
    {
        if (_userId == default)
        {
            throw new UnauthorizedAccessException();
        }
        
        var query = context.Trades
            .AsNoTracking()
            .Where(t=> t.TradingPlanId == TradingPlanId.Of(planId) && _userId == t.CreatedBy);

        if (fromDate.HasValue && toDate.HasValue)
        {
            query = query.Where(t => DateOnly.FromDateTime(t.EntryDateTime) >= fromDate && DateOnly.FromDateTime(t.CloseDateTime) <= toDate);
        }
        else if(fromDate.HasValue)
        {
            query = query.Where(t => DateOnly.FromDateTime(t.EntryDateTime) >= fromDate);
        }
        else if (toDate.HasValue)
        {
            query = query.Where(t => DateOnly.FromDateTime(t.CloseDateTime) <= toDate);
        }
        
        return await query.GroupBy(t=> t.Symbol)
            .Select(g=>new GetGrossAndNetForEachSymbolResult
            {
                Symbol = g.Key,
                NetProfit = Math.Round(g.Sum(t=>t.NetProfit),2),
                GrossProfit = Math.Round(g.Sum(t=>t.GrossProfit),2)
            }).ToListAsync(cancellationToken);
    }

    public async Task<ICollection<GetGrossAndNetForEachSymbolForEachDayOfWeekResult>> GetGrossAndNetForEachSymbolForEachDayOfWeekAsync(Guid planId, Symbols? symbol, DateOnly? fromDate,
        DateOnly? toDate, CancellationToken cancellationToken)
    {
        if (_userId == default)
        {
            throw new UnauthorizedAccessException();
        }
        
        var query = context.Trades
            .AsNoTracking()
            .Where(t=> t.TradingPlanId == TradingPlanId.Of(planId) && _userId == t.CreatedBy);

        if (fromDate.HasValue && toDate.HasValue)
        {
            query = query.Where(t => DateOnly.FromDateTime(t.EntryDateTime) >= fromDate && DateOnly.FromDateTime(t.CloseDateTime) <= toDate);
        }
        else if(fromDate.HasValue)
        {
            query = query.Where(t => DateOnly.FromDateTime(t.EntryDateTime) >= fromDate);
        }
        else if (toDate.HasValue)
        {
            query = query.Where(t => DateOnly.FromDateTime(t.CloseDateTime) <= toDate);
        }

        if (symbol != null)
        {
            query = query.Where(t => t.Symbol == symbol);
        }
        
        return await query.Select(t=>new
            {
                t.GrossProfit,
                t.NetProfit,
                Day = (int)t.EntryDateTime.DayOfWeek
            }).GroupBy(g=> g.Day)
            .OrderBy(g=> g.Key)
            .Select(g=>new GetGrossAndNetForEachSymbolForEachDayOfWeekResult
            {
                DayOfWeek = g.Key,
                NetProfit = Math.Round(g.Sum(t=>t.NetProfit),2),
                GrossProfit = Math.Round(g.Sum(t=>t.GrossProfit),2)
            }).ToListAsync(cancellationToken);
    }
}