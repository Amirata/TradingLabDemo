using BuildingBlocks.Services;

namespace TradingJournal.Api.Tests.Integration;

public class TestCurrentSessionProvider : ICurrentSessionProvider
{
    private readonly Guid? _currentUserId = Guid.Parse("E671C0F9-B5AF-4D75-B2D2-32C00A31A494");

    public Guid? GetUserId() => _currentUserId;
}