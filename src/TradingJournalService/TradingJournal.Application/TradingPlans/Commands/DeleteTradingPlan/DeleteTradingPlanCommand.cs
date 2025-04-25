using FluentValidation;

namespace TradingJournal.Application.TradingPlans.Commands.DeleteTradingPlan;

public class DeleteTradingPlanCommand
    : ICommand<DeleteTradingPlanResult>
{
    public Guid TradingPlanId { get; set; }
}

public class DeleteTradingPlanResult
{
    public bool IsSuccess { get; set; }
}

public class DeleteTradingPlanCommandValidator : AbstractValidator<DeleteTradingPlanCommand>
{
    public DeleteTradingPlanCommandValidator()
    {
        RuleFor(x => x.TradingPlanId).NotEmpty().WithMessage("TradingPlanId is required");
    }
}
