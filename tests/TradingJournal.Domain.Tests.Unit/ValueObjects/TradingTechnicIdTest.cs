using FluentAssertions;
using TradingJournal.Domain.Exceptions;
using TradingJournal.Domain.Models;
using TradingJournal.Domain.ValueObjects;

namespace TradingJournal.Domain.Tests.Unit.ValueObjects;

public class TradingTechnicIdTest
{
    [Fact]
    public void Of_PropertiesFilled_WhenCreateWithOf()
    {
        // Arrange
        var guid = Guid.NewGuid();
        

        // Act
        var tradingTechnicId = TradingTechnicId.Of(guid);


        // Assert
        tradingTechnicId.Value.Should().Be(guid);
       
    }

    [Fact]
    public void Of_DomainException_WhenCreateWithEmptyGuid()
    {
        // Arrange
        var guid = Guid.Empty;

        // Act
        var requestAction = () => TradingTechnicId.Of(guid);
        
        // Assert
        requestAction.Should()
            .Throw<DomainException>();
    }
    
    [Fact]
    public void New_CreateNewGuid_WhenCalledNew()
    {
        // Arrange
        
        // Act
        var tradingTechnicId1 = TradingTechnicId.New();
        var tradingTechnicId2 = TradingTechnicId.New();


        // Assert
        tradingTechnicId1.Value.Should().NotBe(tradingTechnicId2.Value);
       
    }
    
    
}