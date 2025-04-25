using TradingJournal.Application.TradingTechnics.Queries.GetTradingTechnicById;

namespace TradingJournal.Application.TradingPlans.Queries.GetTradingPlanById;

public class GetTradingPlanByIdQuery
    : IQuery<GetTradingPlanByIdResult>
{
    public Guid TradingPlanId { get; set; }
}

public class GetTradingPlanByIdResult
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public TimeOnly? FromTime { get; set; }
    public TimeOnly? ToTime { get; set; }
    public required ICollection<string> SelectedDays { get; set; }
    public required ICollection<GetTradingTechnicByIdResult> Technics { get; set; }
}
