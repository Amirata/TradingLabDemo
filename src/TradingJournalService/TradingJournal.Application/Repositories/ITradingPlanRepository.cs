using BuildingBlocks.Pagination;
using TradingJournal.Application.TradingPlans.Queries.GetTradingPlanById;
using TradingJournal.Application.TradingPlans.Queries.GetTradingPlanByName;
using TradingJournal.Application.TradingPlans.Queries.GetTradingPlans;

namespace TradingJournal.Application.Repositories;
public interface ITradingPlanRepository
{
   Task<bool> CreateAsync(TradingPlan tradingPlan, CancellationToken cancellationToken);
   Task<ICollection<TradingTechnic>> GetTechnicsByIdsAsync(IList<Guid> technicIds,CancellationToken cancellationToken);
   
   Task<TradingPlan?> GetByIdAsync(TradingPlanId id, CancellationToken cancellationToken);
   
   Task<bool> DeleteAsync(TradingPlan tradingPlan, CancellationToken cancellationToken);
   
   Task<bool> UpdateAsync(TradingPlan tradingPlan, CancellationToken cancellationToken);

   Task<GetTradingPlanByIdResult?> GetTradingPlanByIdAsync(Guid id, CancellationToken cancellationToken);
   
   Task<ICollection<GetTradingPlanByNameResult>> GetTradingPlanByNameAsync(string name, CancellationToken cancellationToken);
   
   Task<int> CountAsync(string? search, CancellationToken cancellationToken);
   
   Task<PaginatedResult<GetTradingPlansResult>> GetTradingPlansAsync(GetTradingPlansQuery queryModel,CancellationToken cancellationToken);


}
