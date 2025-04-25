using TradingJournal.Domain.Exceptions;

namespace TradingJournal.Domain.ValueObjects;

public record TradeId
{
    public Guid Value { get; }
    private TradeId(Guid value) => Value = value;

    public static TradeId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new DomainException("TradeId cannot be empty.");
        }

        return new TradeId(value);
    }

    public static TradeId New()
    {
        return new TradeId(Guid.NewGuid());
    }
}