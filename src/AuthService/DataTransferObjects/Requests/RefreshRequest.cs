namespace AuthService.DataTransferObjects.Requests;

public class RefreshRequest
{
    public required string RefreshToken { get; set; }
}