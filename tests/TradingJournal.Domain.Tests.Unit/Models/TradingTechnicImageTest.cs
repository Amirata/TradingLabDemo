using FluentAssertions;
using TradingJournal.Domain.Enums;
using TradingJournal.Domain.Models;
using TradingJournal.Domain.ValueObjects;

namespace TradingJournal.Domain.Tests.Unit.Models;

public class TradingTechnicImageTest
{
    [Fact]
    public void Create_PropertiesFilled_WhenCreate()
    {
        // Arrange
        var path = "path";
        

        // Act
        var tradingTechnicImage = TradingTechnicImage.Create(path);


        // Assert
        tradingTechnicImage.Path.Should().Be(path);
       
    }

    [Fact]
    public void Create_ThrowArgumentException_WhenCreateWithEmptyPath()
    {
        // Arrange
        var path = "";

        // Act
        var requestAction = () => TradingTechnicImage.Create(path);
        
        // Assert
        requestAction.Should()
            .Throw<ArgumentException>();
    }
    
    
}