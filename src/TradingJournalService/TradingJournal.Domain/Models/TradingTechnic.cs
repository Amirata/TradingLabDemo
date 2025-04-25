namespace TradingJournal.Domain.Models;

public class TradingTechnic : Entity<TradingTechnicId>
{
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;

    public UserId UserId { get; set; }

    public User User { get; init; }
    public ICollection<TradingPlan> TradingPlans { get; } = default!;

    private readonly List<TradingTechnicImage> _images = [];

    public IReadOnlyCollection<TradingTechnicImage> Images => _images.AsReadOnly();

    public static TradingTechnic Create(TradingTechnicId id, string name, string description, UserId userId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(description);

        var tradingTechnic = new TradingTechnic
        {
            Id = id,
            Name = name,
            Description = description,
            UserId = userId
        };

        return tradingTechnic;
    }

    public void Update(string name, string description)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(description);

        Name = name;
        Description = description;
    }

    public void AddImage(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        
        var image = TradingTechnicImage.Create(path);
        _images.Add(image);
    }
    
    public void UpdateImage(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        
        var imageExist = _images.FirstOrDefault(x => x.Path == path);

        if (imageExist is null)
        {
            var image = TradingTechnicImage.Create(path);
            _images.Add(image);
        }
    }

    public void RemoveImage(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        
        var image = _images.FirstOrDefault(x => x.Path == path);
        if (image is not null)
        {
            _images.Remove(image);
        }
    }
}