using System.Collections.Immutable;
using System.Linq.Expressions;
using AutoMapper.QueryableExtensions;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Extensions;
using BuildingBlocks.Pagination;
using BuildingBlocks.Services;
using BuildingBlocks.Utilities;
using TradingJournal.Application.Trades.Queries.GetTradeById;
using TradingJournal.Application.Trades.Queries.GetTrades;
using TradingJournal.Infrastructure.Data.Extensions;

namespace TradingJournal.Infrastructure.Data.Repositories;

public class TradeRepository(
    ApplicationDbContext context,
    IMapper mapper,
    ICurrentSessionProvider currentSessionProvider) : ITradeRepository
{
    private readonly Guid? _userId = currentSessionProvider.GetUserId();

    public async Task<bool> CreateAsync(Trade trade, CancellationToken cancellationToken)
    {
        context.Trades.Add(trade);
        return await context.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<Trade?> GetByIdAsync(TradeId id, CancellationToken cancellationToken)
    {
        if (_userId == default)
        {
            throw new UnauthorizedAccessException();
        }

        var trade = await context.Trades
            .FirstOrDefaultAsync(t => t.Id == id && _userId == t.CreatedBy, cancellationToken: cancellationToken);

        return trade;
    }

    public async Task<bool> DeleteAsync(Trade trade, CancellationToken cancellationToken)
    {
        context.Trades.Remove(trade);
        return await context.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> UpdateAsync(Trade trade, CancellationToken cancellationToken)
    {
        context.Trades.Update(trade);
        return await context.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<GetTradeByIdResult?> GetTradeByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        if (_userId == default)
        {
            throw new UnauthorizedAccessException();
        }

        var model = await context.Trades
            .AsNoTracking()
            .Include(t => t.TradingPlan)
            .Where(t => t.Id == TradeId.Of(id) && _userId == t.CreatedBy)
            .ProjectTo<GetTradeByIdResult>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
        return model;
    }

    public async Task<int> CountAsync(string? search, Guid planId, CancellationToken cancellationToken)
    {
        if (_userId == default)
        {
            throw new UnauthorizedAccessException();
        }

        var stringParam = string.IsNullOrEmpty(search) ? "" : search;
        var count = await context.Database.SqlQuery<int>(
            @$"
            SELECT * 
            FROM ""Trades"" 
            WHERE ""TradingPlanId"" = {planId} 
              AND ""CreatedBy"" = {_userId} AND ({stringParam} = '' OR TO_CHAR(""EntryDateTime"", 'YYYY-MM-DD HH24:MI:SS') LIKE '%' || {stringParam} || '%') 
        ").CountAsync(cancellationToken);

        return count;
    }

    public async Task<PaginatedResult<GetTradesResult>> GetTradesAsync(GetTradesQuery queryModel,
        CancellationToken cancellationToken)
    {
        if (_userId == default)
        {
            throw new UnauthorizedAccessException();
        }
        
        var query = context.Trades.AsNoTracking().Where(t=>t.CreatedBy==_userId && t.TradingPlanId == TradingPlanId.Of(queryModel.PaginationRequestWithId.Id)).ProjectTo<GetTradesResult>(mapper.ConfigurationProvider);


        if (!string.IsNullOrWhiteSpace(queryModel.PaginationRequestWithId.Search))
        {
            var search = queryModel.PaginationRequestWithId.Search.Trim();
            //query = query.Where(s => EF.Functions.Like(s.EntryDateTime.ToString("yyyy-MM-dd HH:mm:ss"), $"%{search}%"));
            if (DateTime.TryParse(search, out var parsedDate))
            {
                query = query.Where(s => s.EntryDateTime.Date == parsedDate.Date);
            }
        }

        if (queryModel.PaginationRequestWithId.Sorts?.Any() == true)
        {
            var sortingExpressions = queryModel.PaginationRequestWithId.Sorts.Select(sort => (
                    sort.SortBy switch
                    {
                        nameof(GetTradesResult.EntryDateTime) => (Expression<Func<GetTradesResult, object>>)(p => p.EntryDateTime),
                        nameof(GetTradesResult.CloseDateTime) => p => p.CloseDateTime,
                        nameof(GetTradesResult.NetProfit) => p => p.NetProfit,
                        nameof(GetTradesResult.Pips) => p => p.Pips,
                        nameof(GetTradesResult.Balance) => p => p.Balance,
                        _ => null
                    },
                    sort.SortOrder != null && sort.SortOrder.Equals("asc", StringComparison.OrdinalIgnoreCase))
                )
                .Where(t => t.Item1 != null)
                .ToList();

            query = query.OrderByDynamic(sortingExpressions.ToImmutableArray());
        }

        return await query.ToPaginatedListAsync(queryModel.PaginationRequestWithId.PageNumber,
            queryModel.PaginationRequestWithId.PageSize, cancellationToken);
    }

    public async Task<bool> TradingPlanExistAsync(Guid id, CancellationToken cancellationToken)
    {
        if (_userId == default)
        {
            throw new UnauthorizedAccessException();
        }

        return await context.TradingPlans.AnyAsync(t => t.Id == TradingPlanId.Of(id) && _userId == t.CreatedBy,
            cancellationToken: cancellationToken);
    }
}