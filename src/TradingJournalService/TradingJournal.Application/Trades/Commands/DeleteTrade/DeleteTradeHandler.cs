using TradingJournal.Application.Repositories;

namespace TradingJournal.Application.Trades.Commands.DeleteTrade;
public class DeleteTradeHandler(ITradeRepository repo)
    : ICommandHandler<DeleteTradeCommand, DeleteTradeResult>
{
    public async Task<DeleteTradeResult> Handle(DeleteTradeCommand command, CancellationToken cancellationToken)
    {
        var tradeId = TradeId.Of(command.TradeId);
        
        
        var trade =await repo.GetByIdAsync(tradeId, cancellationToken: cancellationToken);

        if (trade is null)
        {
            throw new NotFoundException(ExceptionMessages.NotFound(nameof(Trade),nameof(command.TradeId),command.TradeId.ToString()));
        }

        var status = await repo.DeleteAsync(trade, cancellationToken);
        
        if (status == false)
        {
            throw new InternalServerException(ExceptionMessages.InternalServerForDelete(nameof(Trade),nameof(trade.Id),trade.Id.Value.ToString()));
        }
       

        return new DeleteTradeResult
        {
            IsSuccess = true
        };        
    }
}
