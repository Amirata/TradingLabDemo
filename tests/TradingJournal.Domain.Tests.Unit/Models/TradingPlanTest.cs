using FluentAssertions;
using TradingJournal.Domain.Enums;
using TradingJournal.Domain.Models;
using TradingJournal.Domain.ValueObjects;

namespace TradingJournal.Domain.Tests.Unit.Models;

public class TradingPlanTest
{
    [Fact]
    public void Create_PropertiesFilled_WhenCreate()
    {
        // Arrange
        var tradingPlanId = TradingPlanId.Of(Guid.Parse("611C9504-8536-48E0-8656-C55989001A41"));
        var userId = UserId.Of(Guid.Parse("611C9504-8536-48E0-8656-C55989001A44"));
        var name = "name";
        var fromTime = new TimeOnly(00, 00, 00);
        var toTime = new TimeOnly(23, 59, 59);
        var daysOfWeek = DaysOfWeek.Monday | DaysOfWeek.Tuesday;
        var formDaySelections = new List<string>
        {
            "Monday",
            "Tuesday"
        };

        // Act
        var tradingPlan = TradingPlan.Create(tradingPlanId, name, fromTime, toTime, formDaySelections,userId);


        // Assert
        tradingPlan.Id.Should().Be(tradingPlanId);
        tradingPlan.Name.Should().Be(name);
        tradingPlan.FromTime.Should().Be(fromTime);
        tradingPlan.ToTime.Should().Be(toTime);
        tradingPlan.SelectedDays.Should().Be(daysOfWeek);
        tradingPlan.Technics.Should().BeEmpty();
    }

    [Fact]
    public void Create_ThrowArgumentException_WhenCreateWithEmptyName()
    {
        // Arrange
        var tradingPlanId = TradingPlanId.Of(Guid.Parse("611C9504-8536-48E0-8656-C55989001A41"));
        var userId = UserId.Of(Guid.Parse("611C9504-8536-48E0-8656-C55989001A44"));
        var name = "";
        var fromTime = new TimeOnly(00, 00, 00);
        var toTime = new TimeOnly(23, 59, 59);
        var formDaySelections = new List<string>
        {
            "Monday",
            "Tuesday"
        };

        // Act
        var requestAction = () => TradingPlan.Create(tradingPlanId, name, fromTime, toTime, formDaySelections,userId);
        
        // Assert
        requestAction.Should()
            .Throw<ArgumentException>();
    }
    
    [Fact]
    public void Update_PropertiesFilled_WhenUpdate()
    {
        // Arrange
        var tradingPlanId = TradingPlanId.Of(Guid.Parse("611C9504-8536-48E0-8656-C55989001A41"));
        var userId = UserId.Of(Guid.Parse("611C9504-8536-48E0-8656-C55989001A44"));
        var name = "name";
        var fromTime = new TimeOnly(00, 00, 00);
        var toTime = new TimeOnly(23, 59, 59);
        var formDaySelections = new List<string>
        {
            "Monday",
            "Tuesday"
        };
        var tradingPlan = TradingPlan.Create(tradingPlanId, name, fromTime, toTime, formDaySelections,userId);
        
        var nameUpdate = "name update";
        var fromTimeUpdate  = new TimeOnly(10, 00, 00);
        var toTimeUpdate  = new TimeOnly(21, 00, 00);
        var daysOfWeekUpdate  = DaysOfWeek.Wednesday | DaysOfWeek.Friday;
        var formDaySelectionsUpdate = new List<string>
        {
            "Wednesday",
            "Friday"
        };

        // Act
        tradingPlan.Update(nameUpdate, fromTimeUpdate, toTimeUpdate, formDaySelectionsUpdate);

        // Assert
        tradingPlan.Id.Should().Be(tradingPlanId);
        tradingPlan.Name.Should().Be(nameUpdate);
        tradingPlan.FromTime.Should().Be(fromTimeUpdate);
        tradingPlan.ToTime.Should().Be(toTimeUpdate);
        tradingPlan.SelectedDays.Should().Be(daysOfWeekUpdate);
        tradingPlan.Technics.Should().BeEmpty();
    }

    [Fact]
    public void Update_ThrowArgumentException_WhenUpdateWithEmptyName()
    {
        // Arrange
        var tradingPlanId = TradingPlanId.Of(Guid.Parse("611C9504-8536-48E0-8656-C55989001A41"));
        var userId = UserId.Of(Guid.Parse("611C9504-8536-48E0-8656-C55989001A44"));
        var name = "name";
        var fromTime = new TimeOnly(00, 00, 00);
        var toTime = new TimeOnly(23, 59, 59);
        var formDaySelections = new List<string>
        {
            "Monday",
            "Tuesday"
        };
        var tradingPlan = TradingPlan.Create(tradingPlanId, name, fromTime, toTime, formDaySelections,userId);
        
        var nameUpdate = "";
        var fromTimeUpdate  = new TimeOnly(10, 00, 00);
        var toTimeUpdate  = new TimeOnly(21, 00, 00);
        var formDaySelectionsUpdate = new List<string>
        {
            "Wednesday",
            "Friday"
        };

        // Act
        var requestAction = () => tradingPlan.Update(nameUpdate, fromTimeUpdate, toTimeUpdate, formDaySelectionsUpdate);
        
        // Assert
        requestAction.Should()
            .Throw<ArgumentException>();
    }
    
    [Fact]
    public void AddTechnic_TechnicAddedToTheList_WhenAddTechnicCall()
    {
        // Arrange
        var tradingPlanId = TradingPlanId.Of(Guid.Parse("611C9504-8536-48E0-8656-C55989001A41"));
        var userId = UserId.Of(Guid.Parse("611C9504-8536-48E0-8656-C55989001A44"));
        var name = "name";
        var fromTime = new TimeOnly(00, 00, 00);
        var toTime = new TimeOnly(23, 59, 59);
        var formDaySelections = new List<string>
        {
            "Monday",
            "Tuesday"
        };
        var tradingPlan = TradingPlan.Create(tradingPlanId, name, fromTime, toTime, formDaySelections,userId);

        var technic = TradingTechnic.Create(TradingTechnicId.Of(Guid.NewGuid()), "Name Technic 1",
            "Description Technic 1",userId);

        // Act
        tradingPlan.AddTechnic(technic);

        // Assert
        tradingPlan.Technics.Should().BeEquivalentTo(new List<TradingTechnic> { technic });
    }
    
    [Fact]
    public void UpdateTechnic_NewTechnicAddedToTheList_WhenItIsNotInTheList()
    {
        // Arrange
        var tradingPlanId = TradingPlanId.Of(Guid.Parse("611C9504-8536-48E0-8656-C55989001A41"));
        var userId = UserId.Of(Guid.Parse("611C9504-8536-48E0-8656-C55989001A44"));
        var name = "name";
        var fromTime = new TimeOnly(00, 00, 00);
        var toTime = new TimeOnly(23, 59, 59);
        var formDaySelections = new List<string>
        {
            "Monday",
            "Tuesday"
        };
        var tradingPlan = TradingPlan.Create(tradingPlanId, name, fromTime, toTime, formDaySelections,userId);

        var technic1 = TradingTechnic.Create(TradingTechnicId.Of(Guid.NewGuid()), "Name Technic 1",
            "Description Technic 1",userId);
        var technic2 = TradingTechnic.Create(TradingTechnicId.Of(Guid.NewGuid()), "Name Technic 2",
            "Description Technic 2",userId);
        tradingPlan.AddTechnic(technic1);

        // Act
        tradingPlan.UpdateTechnic(technic1);
        tradingPlan.UpdateTechnic(technic2);

        // Assert
        tradingPlan.Technics.Should().HaveCount(2);
        tradingPlan.Technics.Should().BeEquivalentTo(new List<TradingTechnic> { technic2, technic1 });
    }
    
    [Fact]
    public void RemoveTechnic_TechnicRemovedFromTheList_WhenItIsInTheList()
    {
        // Arrange
        var tradingPlanId = TradingPlanId.Of(Guid.Parse("611C9504-8536-48E0-8656-C55989001A41"));
        var userId = UserId.Of(Guid.Parse("611C9504-8536-48E0-8656-C55989001A44"));
        var name = "name";
        var fromTime = new TimeOnly(00, 00, 00);
        var toTime = new TimeOnly(23, 59, 59);
        var formDaySelections = new List<string>
        {
            "Monday",
            "Tuesday"
        };
        var tradingPlan = TradingPlan.Create(tradingPlanId, name, fromTime, toTime, formDaySelections,userId);

        var technic1 = TradingTechnic.Create(TradingTechnicId.Of(Guid.Parse("611C9504-8536-48E0-8656-C55989001A42")), "Name Technic 1",
            "Description Technic 1",userId);
        var technic2 = TradingTechnic.Create(TradingTechnicId.Of(Guid.Parse("611C9504-8536-48E0-8656-C55989001A43")), "Name Technic 2",
            "Description Technic 2",userId);
        tradingPlan.AddTechnic(technic1);
        tradingPlan.AddTechnic(technic2);

        // Act
        
        //Exist
        tradingPlan.RemoveTechnic(TradingTechnicId.Of(Guid.Parse("611C9504-8536-48E0-8656-C55989001A43")));
        //Not Exist
        tradingPlan.RemoveTechnic(TradingTechnicId.Of(Guid.Parse("611C9504-8536-48E0-8656-C55989001A44")));

        // Assert
        tradingPlan.Technics.Should().HaveCount(1);
        tradingPlan.Technics.Should().BeEquivalentTo(new List<TradingTechnic> { technic1 });
    }
}