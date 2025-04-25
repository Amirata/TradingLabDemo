using FluentAssertions;
using TradingJournal.Domain.Exceptions;
using TradingJournal.Domain.Models;
using TradingJournal.Domain.ValueObjects;

namespace TradingJournal.Domain.Tests.Unit.ValueObjects;

public class TradingPlanIdTest
{
    [Fact]
    public void Of_PropertiesFilled_WhenCreateWithOf()
    {
        // Arrange
        var guid = Guid.NewGuid();
        

        // Act
        var tradingPlanId = TradingPlanId.Of(guid);


        // Assert
        tradingPlanId.Value.Should().Be(guid);
       
    }

    [Fact]
    public void Of_DomainException_WhenCreateWithEmptyGuid()
    {
        // Arrange
        var guid = Guid.Empty;

        // Act
        var requestAction = () => TradingPlanId.Of(guid);
        
        // Assert
        requestAction.Should()
            .Throw<DomainException>();
    }
    
    [Fact]
    public void New_CreateNewGuid_WhenCalledNew()
    {
        // Arrange
        
        // Act
        var tradingPlanId1 = TradingPlanId.New();
        var tradingPlanId2 = TradingPlanId.New();


        // Assert
        tradingPlanId1.Value.Should().NotBe(tradingPlanId2.Value);
       
    }
    
    
}