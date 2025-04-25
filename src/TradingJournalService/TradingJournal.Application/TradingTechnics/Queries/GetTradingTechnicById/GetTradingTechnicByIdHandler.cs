using TradingJournal.Application.Repositories;

namespace TradingJournal.Application.TradingTechnics.Queries.GetTradingTechnicById;

public class GetTradingTechnicByIdHandler(ITradingTechnicRepository repo)
    : IQueryHandler<GetTradingTechnicByIdQuery, GetTradingTechnicByIdResult>
{
    public async Task<GetTradingTechnicByIdResult> Handle(GetTradingTechnicByIdQuery query,
        CancellationToken cancellationToken)
    {
        var getTradingTechnicByIdResult = await repo.GetTradingTechnicByIdAsync(query.TradingTechnicId, cancellationToken);

        if (getTradingTechnicByIdResult == null)
        {
            throw new NotFoundException(ExceptionMessages.NotFound(nameof(TradingTechnic), nameof(query.TradingTechnicId),
                query.TradingTechnicId.ToString()));
        }

        return getTradingTechnicByIdResult;
    }
}