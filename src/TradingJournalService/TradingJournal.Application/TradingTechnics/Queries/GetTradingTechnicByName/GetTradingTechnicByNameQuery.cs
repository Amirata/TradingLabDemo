namespace TradingJournal.Application.TradingTechnics.Queries.GetTradingTechnicByName;

public class GetTradingTechnicByNameQuery
    : IQuery<ICollection<GetTradingTechnicByNameResult>>
{
    public string Name { get; set; }
}

public class GetTradingTechnicByNameResult
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public ICollection<string> Images { get; set; } = default!;
}