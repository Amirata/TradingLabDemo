namespace AuthService.DataTransferObjects.Results;

public class GetUserResult
{
    public required string Id { get; set; }
    
    public required string UserName { get; set; }
    
    public required string Email { get; set; }
}