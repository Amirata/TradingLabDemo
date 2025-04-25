using FluentValidation;

namespace TradingJournal.Application.TradingTechnics.Commands.DeleteTradingTechnic;

public record DeleteTradingTechnicCommand
    : ICommand<DeleteTradingTechnicResult>
{
    public Guid TradingTechnicId { get; set; }
}

public class DeleteTradingTechnicResult
{
    public bool IsSuccess { get; set; }
}

public class DeleteTradingTechnicCommandValidator : AbstractValidator<DeleteTradingTechnicCommand>
{
    public DeleteTradingTechnicCommandValidator()
    {
        RuleFor(x => x.TradingTechnicId).NotEmpty().WithMessage("TradingTechnicId is required");
    }
}
