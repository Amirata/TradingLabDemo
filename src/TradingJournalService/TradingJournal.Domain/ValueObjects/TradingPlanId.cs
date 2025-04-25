using TradingJournal.Domain.Exceptions;

namespace TradingJournal.Domain.ValueObjects;

public record TradingPlanId
{
    public Guid Value { get; }
    private TradingPlanId(Guid value) => Value = value;

    public static TradingPlanId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new DomainException("TradingPlanId cannot be empty.");
        }

        return new TradingPlanId(value);
    }

    public static TradingPlanId New()
    {
        return new TradingPlanId(Guid.NewGuid());
    }
}