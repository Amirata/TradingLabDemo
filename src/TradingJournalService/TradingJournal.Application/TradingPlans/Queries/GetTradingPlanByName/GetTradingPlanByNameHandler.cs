using TradingJournal.Application.Repositories;

namespace TradingJournal.Application.TradingPlans.Queries.GetTradingPlanByName;

public class GetTradingPlanByNameHandler(ITradingPlanRepository repo)
    : IQueryHandler<GetTradingPlanByNameQuery, ICollection<GetTradingPlanByNameResult>>
{
    public async Task<ICollection<GetTradingPlanByNameResult>> Handle(GetTradingPlanByNameQuery query,
        CancellationToken cancellationToken)
    {
        var getTradingPlanByNameResultList = await repo.GetTradingPlanByNameAsync(query.Name, cancellationToken);
        
        return getTradingPlanByNameResultList;
    }
}