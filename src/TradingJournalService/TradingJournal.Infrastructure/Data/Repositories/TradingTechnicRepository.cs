using System.Collections.Immutable;
using System.Linq.Expressions;
using AutoMapper.QueryableExtensions;
using BuildingBlocks.Extensions;
using BuildingBlocks.Pagination;
using BuildingBlocks.Services;
using TradingJournal.Application.TradingTechnics.Queries.GetTradingTechnicById;
using TradingJournal.Application.TradingTechnics.Queries.GetTradingTechnicByName;
using TradingJournal.Application.TradingTechnics.Queries.GetTradingTechnics;
using TradingJournal.Infrastructure.Data.Extensions;

namespace TradingJournal.Infrastructure.Data.Repositories;

public class TradingTechnicRepository(ApplicationDbContext context, IMapper mapper, ICurrentSessionProvider currentSessionProvider) : ITradingTechnicRepository
{
    private readonly Guid? _userId = currentSessionProvider.GetUserId();
    
    public async Task<bool> CreateAsync(TradingTechnic tradingTechnic, CancellationToken cancellationToken)
    {
        if (_userId == default)
        {
            throw new UnauthorizedAccessException();
        }
        
        if (UserId.Of((Guid)_userId) != tradingTechnic.UserId)
        {
            throw new UnauthorizedAccessException();
        }
        
        context.TradingTechnics.Add(tradingTechnic);
        return await context.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<TradingTechnic?> GetByIdAsync(TradingTechnicId id, CancellationToken cancellationToken)
    {
        if (_userId == default)
        {
            throw new UnauthorizedAccessException();
        }
        
        var tradingTechnic = await context.TradingTechnics//.AsNoTracking()
            .Include(t => t.Images)
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == UserId.Of((Guid)_userId), cancellationToken: cancellationToken);

        return tradingTechnic;
    }

    public async Task<bool> DeleteAsync(TradingTechnic tradingTechnic, CancellationToken cancellationToken)
    {
        context.TradingTechnics.Remove(tradingTechnic);
        return await context.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> UpdateAsync(TradingTechnic tradingTechnic, CancellationToken cancellationToken)
    {
        context.TradingTechnics.Update(tradingTechnic);
        return await context.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<GetTradingTechnicByIdResult?> GetTradingTechnicByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        if (_userId == default)
        {
            throw new UnauthorizedAccessException();
        }
        
        var tradingTechnicDto = await context.TradingTechnics
            .AsNoTracking()
            .Include(t => t.Images)
            .Where(t => t.Id == TradingTechnicId.Of(id) && t.UserId == UserId.Of((Guid)_userId))
            .ProjectTo<GetTradingTechnicByIdResult>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
        return tradingTechnicDto;
    }

    public async Task<ICollection<GetTradingTechnicByNameResult>> GetTradingTechnicByNameAsync(string name,
        CancellationToken cancellationToken)
    {
        if (_userId == default)
        {
            throw new UnauthorizedAccessException();
        }
        
        var model = await context.TradingTechnics
            .AsNoTracking()
            .Include(t => t.Images)
            .Where(t => t.UserId == UserId.Of((Guid)_userId) && t.Name.Contains(name))
            .OrderBy(t => t.Id)
            .ProjectTo<GetTradingTechnicByNameResult>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
        return model;
    }

    public async Task<int> CountAsync(string? search, CancellationToken cancellationToken)
    {
        if (_userId == default)
        {
            throw new UnauthorizedAccessException();
        }
        
        var query = context.TradingTechnics.AsTracking()
            .Where(t=>t.UserId == UserId.Of((Guid)_userId))
            .AsQueryable();
        
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(e => e.Name.Contains(search) || e.Description.Contains(search));
        }
        
        return await query.CountAsync(cancellationToken);
    }

    public async Task<PaginatedResult<GetTradingTechnicsResult>> GetTradingTechnicsAsync(GetTradingTechnicsQuery queryModel,
        CancellationToken cancellationToken)
    {
        if (_userId == default)
        {
            throw new UnauthorizedAccessException();
        }
        
        var query = context.TradingTechnics
            .AsTracking()
            .Where(t=>t.UserId == UserId.Of((Guid)_userId))
            .Include(t => t.Images).ProjectTo<GetTradingTechnicsResult>(mapper.ConfigurationProvider);;

        
        if (!string.IsNullOrWhiteSpace(queryModel.PaginationRequest.Search))
        {
            var search = queryModel.PaginationRequest.Search.Trim();
            query = query.Where(s => 
                EF.Functions.Like(s.Name, $"%{search}%") ||
                EF.Functions.Like(s.Description, $"%{search}%")
            );
        }

        if (queryModel.PaginationRequest.Sorts?.Any() == true)
        {
            var sortingExpressions = queryModel.PaginationRequest.Sorts.Select(sort => (
                    sort.SortBy switch
                    {
                        nameof(GetTradingTechnicsResult.Name) => (Expression<Func<GetTradingTechnicsResult, object>>)(p => p.Name),
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