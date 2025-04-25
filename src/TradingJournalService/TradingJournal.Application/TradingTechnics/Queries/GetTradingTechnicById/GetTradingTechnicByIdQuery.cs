namespace TradingJournal.Application.TradingTechnics.Queries.GetTradingTechnicById;

public class GetTradingTechnicByIdQuery
    : IQuery<GetTradingTechnicByIdResult>
{
    public Guid TradingTechnicId { get; set; }
}

public record GetTradingTechnicByIdResult
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public IEnumerable<string> Images { get; set; } = default!;
}
