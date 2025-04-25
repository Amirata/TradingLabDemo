using TradingJournal.Application.Repositories;

namespace TradingJournal.Application.Trades.Queries.GetTradeById;

public class GetTradeByIdHandler(ITradeRepository repo)
    : IQueryHandler<GetTradeByIdQuery, GetTradeByIdResult>
{
    public async Task<GetTradeByIdResult> Handle(GetTradeByIdQuery query,
        CancellationToken cancellationToken)
    {
        var getTradeByIdResult = await repo.GetTradeByIdAsync(query.TradeId, cancellationToken);

        if (getTradeByIdResult == null)
        {
            throw new NotFoundException(ExceptionMessages.NotFound(nameof(Trade), nameof(query.TradeId),
                query.TradeId.ToString()));
        }

        return getTradeByIdResult;
    }
}