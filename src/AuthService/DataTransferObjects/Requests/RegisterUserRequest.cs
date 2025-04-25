using BuildingBlocks.Auth.Enums;

namespace AuthService.DataTransferObjects.Requests;

public class RegisterUserModel
{
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    
    public required Roles Role { get; set; }
    
}