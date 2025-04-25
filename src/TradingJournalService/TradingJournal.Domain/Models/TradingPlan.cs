namespace TradingJournal.Domain.Models;

public class TradingPlan : Entity<TradingPlanId>
{
    public string Name { get; set; } = default!;
    public TimeOnly? FromTime { get; set; }
    public TimeOnly? ToTime { get; set; }
    public DaysOfWeek SelectedDays { get; set; }

    public UserId UserId { get; set; }

    public User User { get; init; }

    private readonly List<TradingTechnic> _technics = [];
    public IReadOnlyCollection<TradingTechnic> Technics => _technics.AsReadOnly();

    public IReadOnlyCollection<Trade> Trades = default!;

    public static TradingPlan Create(TradingPlanId id, string name, TimeOnly? fromTime, TimeOnly? toTime,
        IEnumerable<string> formDaySelections, UserId userId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        
        var selectedDays = GetSelectedDays(formDaySelections);
        return new TradingPlan()
        {
            Id = id,
            Name = name,
            FromTime = fromTime,
            ToTime = toTime,
            SelectedDays = selectedDays,
            UserId = userId
        };
    }

    public void Update(string name, TimeOnly? fromTime, TimeOnly? toTime, IEnumerable<string> formDaySelections)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        
        var selectedDays = GetSelectedDays(formDaySelections);

        Name = name;
        FromTime = fromTime;
        ToTime = toTime;
        SelectedDays = selectedDays;
    }

    public void AddTechnic(TradingTechnic tradingTechnic)
    {
        _technics.Add(tradingTechnic);
    }

    public void UpdateTechnic(TradingTechnic tradingTechnic)
    {
        var tradingTechnicInTradingPlan = _technics.FirstOrDefault(x => x.Id == tradingTechnic.Id);

        if (tradingTechnicInTradingPlan is null)
        {
            _technics.Add(tradingTechnic);
        }
    }

    public void RemoveTechnic(TradingTechnicId tradingTechnicId)
    {
        var tradingTechnic = _technics.FirstOrDefault(x => x.Id == tradingTechnicId);
        
        if (tradingTechnic is not null)
        {
            _technics.Remove(tradingTechnic);
        }
    }

    private static DaysOfWeek GetSelectedDays(IEnumerable<string> formDaySelections)
    {
        var selectedDays = DaysOfWeek.None;
        foreach (var day in formDaySelections)
        {
            if (Enum.TryParse(day,true, out DaysOfWeek dayEnum))
            {
                selectedDays |= dayEnum; // Add the day using bitwise OR
            }
        }

        return selectedDays;
    }
}