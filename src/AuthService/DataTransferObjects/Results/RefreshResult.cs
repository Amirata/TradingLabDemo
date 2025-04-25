namespace AuthService.DataTransferObjects.Results;

public class RefreshResult
{
    public required string Token { get; set; }
    
    public required string RefreshToken { get; set; }
}