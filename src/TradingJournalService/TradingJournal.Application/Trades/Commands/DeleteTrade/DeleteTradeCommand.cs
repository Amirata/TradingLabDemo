using FluentValidation;

namespace TradingJournal.Application.Trades.Commands.DeleteTrade;

public class DeleteTradeCommand
    : ICommand<DeleteTradeResult>
{
    public Guid TradeId { get; set; }
}

public class DeleteTradeResult
{
    public bool IsSuccess { get; set; }
}

public class DeleteTradeCommandValidator : AbstractValidator<DeleteTradeCommand>
{
    public DeleteTradeCommandValidator()
    {
        RuleFor(x => x.TradeId).NotEmpty().WithMessage("TradeId is required");
    }
}
