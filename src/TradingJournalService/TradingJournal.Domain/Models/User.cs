namespace TradingJournal.Domain.Models;

public class User : Entity<UserId>
{
    public string Name { get;  set; } = default!;

    public ICollection<TradingPlan> TradingPlans { get; set; }
    
    public ICollection<TradingTechnic> TradingTechnic { get; set; }


    public static User Create(UserId id, string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return new User()
        {
            Id = id,
            Name = name
        };
    }

    public void Update(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Name = name;
    }
}