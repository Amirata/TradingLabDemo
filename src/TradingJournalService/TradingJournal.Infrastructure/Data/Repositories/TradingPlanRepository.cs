using System.Collections.Immutable;
using System.Linq.Expressions;
using AutoMapper.QueryableExtensions;
using BuildingBlocks.Extensions;
using BuildingBlocks.Pagination;
using BuildingBlocks.Services;
using TradingJournal.Application.TradingPlans.Queries.GetTradingPlanById;
using TradingJournal.Application.TradingPlans.Queries.GetTradingPlanByName;
using TradingJournal.Application.TradingPlans.Queries.GetTradingPlans;
using TradingJournal.Infrastructure.Data.Extensions;

namespace TradingJournal.Infrastructure.Data.Repositories;

public class TradingPlanRepository(ApplicationDbContext context, IMapper mapper, ICurrentSessionProvider currentSessionProvider) : ITradingPlanRepository
{
    private readonly Guid? _userId = currentSessionProvider.GetUserId();
    
    public async Task<bool> CreateAsync(TradingPlan tradingPlan, CancellationToken cancellationToken)
    {
        if (_userId == default)
        {
            throw new UnauthorizedAccessException();
        }
        
        if (UserId.Of((Guid)_userId) != tradingPlan.UserId)
        {
            throw new UnauthorizedAccessException();
        }
        
        context.TradingPlans.Add(tradingPlan);
        return await context.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<ICollection<TradingTechnic>> GetTechnicsByIdsAsync(IList<Guid> technicIds,
        CancellationToken cancellationToken)
    {
        if (_userId == default)
        {
            throw new UnauthorizedAccessException();
        }
        
        var tradingTechnicIds = technicIds.Select(t => TradingTechnicId.Of(t)).ToList();
        var technics = await context
            .TradingTechnics
            .Where(t => t.UserId == UserId.Of((Guid)_userId) && tradingTechnicIds.Contains(t.Id))
            .ToListAsync(cancellationToken: cancellationToken);

        return technics;
    }

    public async Task<TradingPlan?> GetByIdAsync(TradingPlanId id, CancellationToken cancellationToken)
    {
        if (_userId == default)
        {
            throw new UnauthorizedAccessException();
        }
        
        var tradingPlan = await context.TradingPlans
            .Include(t => t.Technics)
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == UserId.Of((Guid)_userId), cancellationToken: cancellationToken);
        return tradingPlan;
    }

    public async Task<bool> DeleteAsync(TradingPlan tradingPlan, CancellationToken cancellationToken)
    {
        context.TradingPlans.Remove(tradingPlan);
        return await context.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> UpdateAsync(TradingPlan tradingPlan, CancellationToken cancellationToken)
    {
        context.TradingPlans.Update(tradingPlan);
        return await context.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<GetTradingPlanByIdResult?> GetTradingPlanByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        if (_userId == default)
        {
            throw new UnauthorizedAccessException();
        }
        
        var model = await context.TradingPlans
            .AsNoTracking()
            .Include(t => t.Technics)
            .Where(t => t.Id == TradingPlanId.Of(id) && t.UserId == UserId.Of((Guid)_userId))
            .ProjectTo<GetTradingPlanByIdResult>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
        return model;
    }

    public async Task<ICollection<GetTradingPlanByNameResult>> GetTradingPlanByNameAsync(string name,
        CancellationToken cancellationToken)
    {
        if (_userId == default)
        {
            throw new UnauthorizedAccessException();
        }
        
        var model = await context.TradingPlans
            .AsNoTracking()
            .Include(t => t.Technics)
            .Where(t => t.UserId == UserId.Of((Guid)_userId) && t.Name.Contains(name))
            .OrderBy(t => t.Id)
            .ProjectTo<GetTradingPlanByNameResult>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
        return model;
    }

    public async Task<int> CountAsync(string? search, CancellationToken cancellationToken)
    {
        if (_userId == default)
        {
            throw new UnauthorizedAccessException();
        }

        var query = context.TradingPlans.AsTracking().Where(t=>t.UserId == UserId.Of((Guid)_userId))
            .AsQueryable();
            
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(e => e.Name.Contains(search));
        }
        
        return await query.CountAsync(cancellationToken);
    }

    public async Task<PaginatedResult<GetTradingPlansResult>> GetTradingPlansAsync(GetTradingPlansQuery queryModel,
        CancellationToken cancellationToken)
    {
        if (_userId == default)
        {
            throw new UnauthorizedAccessException();
        }
        
        var query = context.TradingPlans
            .AsTracking()
            .Where(t=>t.UserId == UserId.Of((Guid)_userId))
            .Include(t => t.Technics).ProjectTo<GetTradingPlansResult>(mapper.ConfigurationProvider);;

        
        if (!string.IsNullOrWhiteSpace(queryModel.PaginationRequest.Search))
        {
            var search = queryModel.PaginationRequest.Search.Trim();
            query = query.Where(s => EF.Functions.Like(s.Name, $"%{search}%"));
           
        }

        if (queryModel.PaginationRequest.Sorts?.Any() == true)
        {
            var sortingExpressions = queryModel.PaginationRequest.Sorts.Select(sort => (
                    sort.SortBy switch
                    {
                        nameof(GetTradingPlansResult.Name) => (Expression<Func<GetTradingPlansResult, object>>)(p => p.Name),
                        nameof(GetTradingPlansResult.FromTime) => (Expression<Func<GetTradingPlansResult, object>>)(p => p.FromTime ?? new TimeOnly()),
                        nameof(GetTradingPlansResult.ToTime) => (Expression<Func<GetTradingPlansResult, object>>)(p => p.ToTime ?? new TimeOnly()),
                        _ => null
                    },
                    sort.SortOrder != null && sort.SortOrder.Equals("asc", StringComparison.OrdinalIgnoreCase))
                )
                .Where(t => t.Item1 != null)
                .ToList();

            query = query.OrderByDynamic(sortingExpressions.ToImmutableArray());
        }

        return await query.ToPaginatedListAsync(queryModel.PaginationRequest.PageNumber,
            queryModel.PaginationRequest.PageSize, cancellationToken);
    }
    
    
}