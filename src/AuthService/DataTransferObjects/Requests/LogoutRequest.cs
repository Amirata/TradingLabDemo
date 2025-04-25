namespace AuthService.DataTransferObjects.Requests;

public class LogoutRequest
{
    public required string RefreshToken { get; set; }
}