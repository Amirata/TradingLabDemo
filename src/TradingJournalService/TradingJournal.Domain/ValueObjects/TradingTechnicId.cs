using TradingJournal.Domain.Exceptions;

namespace TradingJournal.Domain.ValueObjects;
public record TradingTechnicId
{
    public Guid Value { get; }
    private TradingTechnicId(Guid value) => Value = value;
    public static TradingTechnicId Of(Guid value)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (value == Guid.Empty)
        {
            throw new DomainException("TradingTechnicId cannot be empty.");
        }

        return new TradingTechnicId(value);
    }

    public static TradingTechnicId New()
    {
        return new TradingTechnicId(Guid.NewGuid());
    }
}
