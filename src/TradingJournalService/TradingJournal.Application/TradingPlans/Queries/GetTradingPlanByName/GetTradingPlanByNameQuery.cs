using TradingJournal.Application.TradingTechnics.Queries.GetTradingTechnicById;

namespace TradingJournal.Application.TradingPlans.Queries.GetTradingPlanByName;

public class GetTradingPlanByNameQuery
    : IQuery<ICollection<GetTradingPlanByNameResult>>
{
    public string Name { get; set; }
}

public class GetTradingPlanByNameResult
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public TimeOnly? FromTime { get; set; }
    public TimeOnly? ToTime { get; set; }
    public required ICollection<string> SelectedDays { get; set; }
    public required ICollection<GetTradingTechnicByIdResult> Technics { get; set; }
}