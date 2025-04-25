using TradingJournal.Application.Repositories;

namespace TradingJournal.Application.TradingPlans.Queries.GetTradingPlanById;

public class GetTradingPlanByIdHandler(ITradingPlanRepository repo)
    : IQueryHandler<GetTradingPlanByIdQuery, GetTradingPlanByIdResult>
{
    public async Task<GetTradingPlanByIdResult> Handle(GetTradingPlanByIdQuery query,
        CancellationToken cancellationToken)
    {
        var getTradingPlanByIdResult = await repo.GetTradingPlanByIdAsync(query.TradingPlanId, cancellationToken);

        if (getTradingPlanByIdResult == null)
        {
            throw new NotFoundException(ExceptionMessages.NotFound(nameof(TradingPlan), nameof(query.TradingPlanId),
                query.TradingPlanId.ToString()));
        }

        return getTradingPlanByIdResult;
    }
}