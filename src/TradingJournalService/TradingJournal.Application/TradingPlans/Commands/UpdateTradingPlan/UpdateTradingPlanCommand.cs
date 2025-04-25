using FluentValidation;

namespace TradingJournal.Application.TradingPlans.Commands.UpdateTradingPlan;

public class UpdateTradingPlanCommand
    : ICommand<UpdateTradingPlanResult>
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public TimeOnly? FromTime { get; set; }
    public TimeOnly? ToTime { get; set; }
    public required IEnumerable<string> SelectedDays { get; set; }
    public required IList<Guid> Technics { get; set; }
    public required IList<Guid> RemovedTechnics { get; set; }
}

public class UpdateTradingPlanResult
{
    public bool IsSuccess { get; set; }
}

public class UpdateTradingPlanCommandValidator : AbstractValidator<UpdateTradingPlanCommand>
{
    public UpdateTradingPlanCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required.");
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
        RuleFor(x => x.Technics).NotNull().WithMessage("Technics is required.");
        RuleFor(x => x.FromTime).LessThan(x=>x.ToTime).WithMessage("ToTime must be greater than FromTime.");
        RuleFor(x => x).Must(t=>!t.Technics.Any(r=>t.RemovedTechnics.Contains(r))).WithMessage("The Technics contains values that are also present in the RemovedTechnics.");
        RuleFor(x => x).Must(t=>
            (t.FromTime==null && t.ToTime==null) ||
            (t.FromTime!=null && t.ToTime!=null)
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

