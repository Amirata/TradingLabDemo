namespace AuthService.DataTransferObjects.Requests;

public class UpdateUserModel
{
    public required string UserName { get; set; }
    public required string Email { get; set; }
}