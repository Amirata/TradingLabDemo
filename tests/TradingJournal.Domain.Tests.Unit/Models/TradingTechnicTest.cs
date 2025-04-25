using FluentAssertions;
using TradingJournal.Domain.Enums;
using TradingJournal.Domain.Models;
using TradingJournal.Domain.ValueObjects;

namespace TradingJournal.Domain.Tests.Unit.Models;

public class TradingTechnicTest
{
    [Fact]
    public void Create_PropertiesFilled_WhenCreate()
    {
        // Arrange
        var tradingTechnicId = TradingTechnicId.Of(Guid.Parse("611C9504-8536-48E0-8656-C55989001A41"));
        var userId = UserId.Of(Guid.Parse("611C9504-8536-48E0-8656-C55989001A44"));
        var name = "name";
        var description = "description";

        // Act
        var tradingTechnic = TradingTechnic.Create(tradingTechnicId, name, description, userId);


        // Assert
        tradingTechnic.Id.Should().Be(tradingTechnicId);
        tradingTechnic.Name.Should().Be(name);
        tradingTechnic.Description.Should().Be(description);
    }

    [Fact]
    public void Create_ThrowArgumentException_WhenCreateWithEmptyName()
    {
        // Arrange
        var tradingTechnicId = TradingTechnicId.Of(Guid.Parse("611C9504-8536-48E0-8656-C55989001A41"));
        var userId = UserId.Of(Guid.Parse("611C9504-8536-48E0-8656-C55989001A44"));
        var name = "";
        var description = "description";

        // Act
        var requestAction = () => TradingTechnic.Create(tradingTechnicId, name, description, userId);

        // Assert
        requestAction.Should()
            .Throw<ArgumentException>();
    }

    [Fact]
    public void Create_ThrowArgumentException_WhenCreateWithEmptyDescription()
    {
        // Arrange
        var tradingTechnicId = TradingTechnicId.Of(Guid.Parse("611C9504-8536-48E0-8656-C55989001A41"));
        var userId = UserId.Of(Guid.Parse("611C9504-8536-48E0-8656-C55989001A44"));
        var name = "name";
        var description = "";

        // Act
        var requestAction = () => TradingTechnic.Create(tradingTechnicId, name, description, userId);

        // Assert
        requestAction.Should()
            .Throw<ArgumentException>();
    }

    [Fact]
    public void Update_PropertiesFilled_WhenUpdate()
    {
        // Arrange
        var tradingTechnicId = TradingTechnicId.Of(Guid.Parse("611C9504-8536-48E0-8656-C55989001A41"));
        var userId = UserId.Of(Guid.Parse("611C9504-8536-48E0-8656-C55989001A44"));
        var name = "name";
        var description = "description";
        var tradingTechnic = TradingTechnic.Create(tradingTechnicId, name, description, userId);

        var nameUpdate = "name update";
        var descriptionUpdate = "description update";

        // Act
        tradingTechnic.Update(nameUpdate, descriptionUpdate);

        // Assert
        tradingTechnic.Id.Should().Be(tradingTechnicId);
        tradingTechnic.Name.Should().Be(nameUpdate);
        tradingTechnic.Description.Should().Be(descriptionUpdate);
    }

    [Fact]
    public void Update_ThrowArgumentException_WhenUpdateWithEmptyName()
    {
        // Arrange
        var tradingTechnicId = TradingTechnicId.Of(Guid.Parse("611C9504-8536-48E0-8656-C55989001A41"));
        var userId = UserId.Of(Guid.Parse("611C9504-8536-48E0-8656-C55989001A44"));
        var name = "name";
        var description = "description";
        var tradingTechnic = TradingTechnic.Create(tradingTechnicId, name, description, userId);

        var nameUpdate = "";
        var descriptionUpdate = "description update";

        // Act
        var requestAction = () => tradingTechnic.Update(nameUpdate, descriptionUpdate);

        // Assert
        requestAction.Should()
            .Throw<ArgumentException>();
    }

    [Fact]
    public void Update_ThrowArgumentException_WhenUpdateWithEmptyDescription()
    {
        // Arrange
        var tradingTechnicId = TradingTechnicId.Of(Guid.Parse("611C9504-8536-48E0-8656-C55989001A41"));
        var userId = UserId.Of(Guid.Parse("611C9504-8536-48E0-8656-C55989001A44"));
        var name = "name";
        var description = "description";
        var tradingTechnic = TradingTechnic.Create(tradingTechnicId, name, description, userId);

        var nameUpdate = "name update";
        var descriptionUpdate = "";

        // Act
        var requestAction = () => tradingTechnic.Update(nameUpdate, descriptionUpdate);

        // Assert
        requestAction.Should()
            .Throw<ArgumentException>();
    }

    [Fact]
    public void AddImage_ImageAddedToTheList_WhenAddImageCall()
    {
        // Arrange
        var tradingTechnicId = TradingTechnicId.Of(Guid.Parse("611C9504-8536-48E0-8656-C55989001A41"));
        var userId = UserId.Of(Guid.Parse("611C9504-8536-48E0-8656-C55989001A44"));
        var name = "name";
        var description = "description";
        var tradingTechnic = TradingTechnic.Create(tradingTechnicId, name, description, userId);
        var ImagePath = "ImagePath";

        // Act
        tradingTechnic.AddImage(ImagePath);

        // Assert
        tradingTechnic.Images.Should().HaveCount(1);
        tradingTechnic.Images.Should().Contain(x => x.Path == ImagePath);
    }

    [Fact]
    public void AddImage_ThrowArgumentException_WhenPathNullOrEmpty()
    {
        // Arrange
        var tradingTechnicId = TradingTechnicId.Of(Guid.Parse("611C9504-8536-48E0-8656-C55989001A41"));
        var userId = UserId.Of(Guid.Parse("611C9504-8536-48E0-8656-C55989001A44"));
        var name = "name";
        var description = "description";
        var tradingTechnic = TradingTechnic.Create(tradingTechnicId, name, description, userId);
        var ImagePath = "";

        // Act
        var requestAction = () => tradingTechnic.AddImage(ImagePath);

        // Assert
        requestAction.Should()
            .Throw<ArgumentException>();
    }

    [Fact]
    public void UpdateImage_ImageAddedToTheList_WhenItIsNotInTheList()
    {
        // Arrange
        var tradingTechnicId = TradingTechnicId.Of(Guid.Parse("611C9504-8536-48E0-8656-C55989001A41"));
        var userId = UserId.Of(Guid.Parse("611C9504-8536-48E0-8656-C55989001A44"));
        var name = "name";
        var description = "description";
        var tradingTechnic = TradingTechnic.Create(tradingTechnicId, name, description, userId);
        var ImagePath1 = "ImagePath 1";
        var ImagePath2 = "ImagePath 2";

        tradingTechnic.AddImage(ImagePath1);

        // Act
        tradingTechnic.UpdateImage(ImagePath1);
        tradingTechnic.UpdateImage(ImagePath2);

        // Assert
        tradingTechnic.Images.Should().HaveCount(2);
        tradingTechnic.Images.Should().Contain(x => x.Path == ImagePath1);
        tradingTechnic.Images.Should().Contain(x => x.Path == ImagePath2);
    }

    [Fact]
    public void UpdateImage_ThrowArgumentException_WhenPathNullOrEmpty()
    {
        // Arrange
        var tradingTechnicId = TradingTechnicId.Of(Guid.Parse("611C9504-8536-48E0-8656-C55989001A41"));
        var userId = UserId.Of(Guid.Parse("611C9504-8536-48E0-8656-C55989001A44"));
        var name = "name";
        var description = "description";
        var tradingTechnic = TradingTechnic.Create(tradingTechnicId, name, description, userId);
        var ImagePath1 = "ImagePath 1";
        var ImagePath2 = "";

        tradingTechnic.AddImage(ImagePath1);

        // Act
        var requestAction = () => tradingTechnic.UpdateImage(ImagePath2);

        // Assert
        requestAction.Should()
            .Throw<ArgumentException>();
    }

    [Fact]
    public void RemoveImage_ImageRemovedFromTheList_WhenItIsInTheList()
    {
        // Arrange
        var tradingTechnicId = TradingTechnicId.Of(Guid.Parse("611C9504-8536-48E0-8656-C55989001A41"));
        var userId = UserId.Of(Guid.Parse("611C9504-8536-48E0-8656-C55989001A44"));
        var name = "name";
        var description = "description";
        var tradingTechnic = TradingTechnic.Create(tradingTechnicId, name, description, userId);
        var ImagePath1 = "ImagePath 1";
        var ImagePath2 = "ImagePath 2";

        tradingTechnic.AddImage(ImagePath1);
        tradingTechnic.AddImage(ImagePath2);

        // Act
        tradingTechnic.RemoveImage(ImagePath1);

        // Assert
        tradingTechnic.Images.Should().HaveCount(1);
        tradingTechnic.Images.Should().NotContain(x => x.Path == ImagePath1);
        tradingTechnic.Images.Should().Contain(x => x.Path == ImagePath2);
    }

    [Fact]
    public void RemoveImage_ThrowArgumentException_WhenPathNullOrEmpty()
    {
        // Arrange
        var tradingTechnicId = TradingTechnicId.Of(Guid.Parse("611C9504-8536-48E0-8656-C55989001A41"));
        var userId = UserId.Of(Guid.Parse("611C9504-8536-48E0-8656-C55989001A44"));
        var name = "name";
        var description = "description";
        var tradingTechnic = TradingTechnic.Create(tradingTechnicId, name, description, userId);
        var ImagePath1 = "ImagePath 1";
        var ImagePath2 = "";

        tradingTechnic.AddImage(ImagePath1);

        // Act
        var requestAction = () => tradingTechnic.RemoveImage(ImagePath2);

        // Assert
        requestAction.Should()
            .Throw<ArgumentException>();
    }
}