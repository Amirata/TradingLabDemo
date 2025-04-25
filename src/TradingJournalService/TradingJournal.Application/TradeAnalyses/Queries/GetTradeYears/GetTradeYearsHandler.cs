using TradingJournal.Application.Repositories;


namespace TradingJournal.Application.TradeAnalyses.Queries.GetTradeYears;

public class GetTradeYearsHandler(ITradeAnalyseRepository repo)
    : IQueryHandler<GetTradeYearsQuery, ICollection<int>>
{
    public async Task<ICollection<int>> Handle(GetTradeYearsQuery query,
        CancellationToken cancellationToken)
    {
        var getTradeYearsResultList = await repo.GetTradeYearsAsync(query.PlanId, cancellationToken);
        
        return getTradeYearsResultList;
    }
}