using TradingJournal.Application.Repositories;

namespace TradingJournal.Application.Trades.Commands.UpdateTrade;

public class UpdateTradeHandler(ITradeRepository repo)
    : ICommandHandler<UpdateTradeCommand, UpdateTradeResult>
{
    public async Task<UpdateTradeResult> Handle(UpdateTradeCommand command,
        CancellationToken cancellationToken)
    {
        var tradeId = TradeId.Of(command.Id);

        var trade = await repo.GetByIdAsync(tradeId, cancellationToken);

        if (trade is null)
        {
            throw new NotFoundException(ExceptionMessages.NotFound(nameof(Trade),
                nameof(command.Id), command.Id.ToString()));
        }

        UpdateTradeWithNewValues(trade, command);

        var status = await repo.UpdateAsync(trade, cancellationToken);

        if (status == false)
        {
            throw new InternalServerException(ExceptionMessages.InternalServerForUpdate(nameof(Trade),
                nameof(trade.Id), trade.Id.Value.ToString()));
        }

        return new UpdateTradeResult
        {
            IsSuccess = true
        };
    }

    private void UpdateTradeWithNewValues(Trade trade,
        UpdateTradeCommand updateTradeCommand)
    {
        trade.Update(
            updateTradeCommand.Symbol,
            updateTradeCommand.PositionType,
            updateTradeCommand.Volume,
            updateTradeCommand.EntryPrice,
            updateTradeCommand.ClosePrice,
            updateTradeCommand.StopLossPrice,
            updateTradeCommand.EntryDateTime,
            updateTradeCommand.CloseDateTime,
            updateTradeCommand.Commission,
            updateTradeCommand.Swap,
            updateTradeCommand.Pips,
            updateTradeCommand.NetProfit,
            updateTradeCommand.GrossProfit,
            updateTradeCommand.Balance
        );
    }
}