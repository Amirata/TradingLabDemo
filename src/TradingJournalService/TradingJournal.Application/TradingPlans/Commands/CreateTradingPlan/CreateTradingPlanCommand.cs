using FluentValidation;

namespace TradingJournal.Application.TradingPlans.Commands.CreateTradingPlan;

public class CreateTradingPlanCommand : ICommand<CreateTradingPlanResult>
{
    public required string Name { get; set; }
   
    public TimeOnly? FromTime { get; set; }
    
    public TimeOnly? ToTime { get; set; }
    public IEnumerable<string> SelectedDays { get; set; } = default!;
    public IList<Guid> Technics { get; set; } = default!;

    public Guid UserId { get; set; }
}

public class CreateTradingPlanResult
{
    public Guid Id { get; set; }   
}

public class CreateTradingPlanCommandValidator : AbstractValidator<CreateTradingPlanCommand>
{
    public CreateTradingPlanCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
        RuleFor(x => x.Technics).NotEmpty().WithMessage("Technics is required.");
        RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required.");
        RuleFor(x => x.FromTime).LessThan(x => x.ToTime)
            .WithMessage("ToTime must be greater than FromTime.");
        RuleFor(x => x).Must(t =>
            (t.FromTime == null && t.ToTime == null) ||
            (t.FromTime != null && t.ToTime != null)
        ).WithMessage("Both FromTime and ToTime must either be null or have values.");
        RuleFor(x => x).Must(t => CheckSelectedDays(t.SelectedDays)).WithMessage("Selected days is not valid.");
    }

    private static bool CheckSelectedDays(IEnumerable<string> days)
    {
        foreach (var day in days)
        {
            if (!Enum.TryParse(typeof(DayOfWeek), day, true, out _))
            {
                return false;
            }
        }

        return true;
    }
}