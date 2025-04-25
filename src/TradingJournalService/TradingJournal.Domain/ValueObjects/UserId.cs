using TradingJournal.Domain.Exceptions;

namespace TradingJournal.Domain.ValueObjects;

public record UserId
{
    public Guid Value { get; }
    private UserId(Guid value) => Value = value;

    public static UserId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new DomainException("UserId cannot be empty.");
        }

        return new UserId(value);
    }
    
}