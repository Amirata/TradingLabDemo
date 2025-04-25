using TradingJournal.Application.Repositories;

namespace TradingJournal.Application.Trades.Commands.CreateTrade;

public class CreateTradeHandler(ITradeRepository repo)
    : ICommandHandler<CreateTradeCommand, CreateTradeResult>
{
    public async Task<CreateTradeResult> Handle(CreateTradeCommand command,
        CancellationToken cancellationToken)
    {
        var trade = await CreateNewTrade(command, cancellationToken);

        var status = await repo.CreateAsync(trade, cancellationToken);
        
        if (status == false)
        {
            throw new InternalServerException(ExceptionMessages.InternalServerForCreate(nameof(Trade),nameof(trade.Id),trade.Id.Value.ToString()));
        }

        return new CreateTradeResult { Id = trade.Id.Value };
    }

    private async Task<Trade> CreateNewTrade(CreateTradeCommand createTradeCommand, CancellationToken cancellationToken)
    {
        var tradingPlanExist = await repo.TradingPlanExistAsync(createTradeCommand.TradingPlanId, cancellationToken);
        
        if (!tradingPlanExist)
        {
            throw new BadRequestException(ExceptionMessages.BadRequest(nameof(createTradeCommand.TradingPlanId)));
        }
        var newTrade = Trade.Create(
            TradeId.New(),
            createTradeCommand.Symbol,
            createTradeCommand.PositionType,
            createTradeCommand.Volume,
            createTradeCommand.EntryPrice,
            createTradeCommand.ClosePrice,
            createTradeCommand.StopLossPrice,
            createTradeCommand.EntryDateTime,
            createTradeCommand.CloseDateTime,
            createTradeCommand.Commission,
            createTradeCommand.Swap,
            createTradeCommand.Pips,
            createTradeCommand.NetProfit,
            createTradeCommand.GrossProfit,
            createTradeCommand.Balance,
            TradingPlanId.Of(createTradeCommand.TradingPlanId)
        );
        

        return newTrade;
    }
}