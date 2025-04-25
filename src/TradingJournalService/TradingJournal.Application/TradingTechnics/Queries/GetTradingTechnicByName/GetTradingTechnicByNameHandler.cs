using TradingJournal.Application.Repositories;
using TradingJournal.Application.TradingTechnics.Queries.GetTradingTechnicByName;

namespace TradingJournal.Application.TradingTechnics.Queries.GetTradingTechnicByName;

public class GetTradingTechnicByNameHandler(ITradingTechnicRepository repo)
    : IQueryHandler<GetTradingTechnicByNameQuery, ICollection<GetTradingTechnicByNameResult>>
{
    public async Task<ICollection<GetTradingTechnicByNameResult>> Handle(GetTradingTechnicByNameQuery query,
        CancellationToken cancellationToken)
    {
        var getTradingTechnicByNameResult = await repo.GetTradingTechnicByNameAsync(query.Name, cancellationToken);

        if (getTradingTechnicByNameResult == null)
        {
            throw new NotFoundException(ExceptionMessages.NotFound(nameof(TradingTechnic), nameof(query.Name),
                query.Name));
        }

        return getTradingTechnicByNameResult;
    }
}