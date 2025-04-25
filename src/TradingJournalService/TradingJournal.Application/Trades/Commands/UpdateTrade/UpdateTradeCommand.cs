using FluentValidation;
using TradingJournal.Domain.Enums;

namespace TradingJournal.Application.Trades.Commands.UpdateTrade;

public class UpdateTradeCommand
    : ICommand<UpdateTradeResult>
{
    public Guid Id { get; set; }
    public Symbols Symbol { get;  set; }
    public PositionType PositionType { get;  set; }
    public double Volume { get;  set; }
    public double EntryPrice { get;  set; }
    public double ClosePrice { get;  set; }
    public double StopLossPrice { get;  set; }
    public DateTime EntryDateTime { get;  set; }
    public DateTime CloseDateTime { get;  set; }
    public double Commission { get;  set; }
    public double Swap { get;  set; }
    public double Pips { get;  set; }
    public double NetProfit { get;  set; }
    public double GrossProfit { get;  set; }
    public double Balance { get;  set; }
}

public class UpdateTradeResult
{
    public bool IsSuccess { get; set; }
}

public class UpdateTradeCommandValidator : AbstractValidator<UpdateTradeCommand>
{
    public UpdateTradeCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required.");
        RuleFor(x => x.EntryDateTime).NotEqual(DateTime.MinValue).WithMessage("EntryDateTime is required.");
        RuleFor(x => x.CloseDateTime).NotEqual(DateTime.MinValue).WithMessage("CloseDateTime is required.");
        RuleFor(x => x.Volume).GreaterThan(0).WithMessage("Volume Must be greater than 0.");
        RuleFor(x => x.EntryPrice).GreaterThan(0).WithMessage("EntryPrice Must be greater than 0.");
        RuleFor(x => x.ClosePrice).GreaterThan(0).WithMessage("ClosePrice Must be greater than 0.");
        RuleFor(x => x.StopLossPrice).GreaterThan(0).WithMessage("StopLossPrice Must be greater than 0.");
        RuleFor(x => x.Commission).LessThanOrEqualTo(0).WithMessage("Commission Must be negative.");
        RuleFor(x => x.Balance).GreaterThanOrEqualTo(0).WithMessage("Balance Must be positive.");
    }
   
}

