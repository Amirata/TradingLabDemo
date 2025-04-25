using BuildingBlocks.Pagination;
using TradingJournal.Application.TradingTechnics.Queries.GetTradingTechnicById;
using TradingJournal.Application.TradingTechnics.Queries.GetTradingTechnicByName;
using TradingJournal.Application.TradingTechnics.Queries.GetTradingTechnics;

namespace TradingJournal.Application.Repositories;

public interface ITradingTechnicRepository
{
    Task<bool> CreateAsync(TradingTechnic tradingTechnic, CancellationToken cancellationToken);

    Task<TradingTechnic?> GetByIdAsync(TradingTechnicId id, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(TradingTechnic tradingTechnic, CancellationToken cancellationToken);

    Task<bool> UpdateAsync(TradingTechnic tradingTechnic, CancellationToken cancellationToken);
    
    Task<GetTradingTechnicByIdResult?> GetTradingTechnicByIdAsync(Guid id, CancellationToken cancellationToken);
   
    Task<ICollection<GetTradingTechnicByNameResult>> GetTradingTechnicByNameAsync(string name, CancellationToken cancellationToken);
   
    Task<int> CountAsync(string? search, CancellationToken cancellationToken);
   
    Task<PaginatedResult<GetTradingTechnicsResult>> GetTradingTechnicsAsync(GetTradingTechnicsQuery queryModel,CancellationToken cancellationToken);

}