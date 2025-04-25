namespace TradingJournal.Domain.Models;

public class TradingTechnicImage : Entity<long>
{
    public string Path { get; set; } = default!;

    public TradingTechnicId TradingTechnicId { get; set; } = default!;


    public static TradingTechnicImage Create(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        return new TradingTechnicImage { Path = path};
    }
}