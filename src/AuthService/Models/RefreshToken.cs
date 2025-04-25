namespace AuthService.Models;

public class RefreshToken
{
    public int Id { get; set; }
    public string Token { get; set; } = default!;         
    public DateTime ExpiresAt { get; set; }    
    public bool IsRevoked { get; set; }     
    public DateTime CreatedAt { get; set; } 
    public DateTime? RevokedAt { get; set; } 
    
    public string UserId { get; set; }  = default!;
    public ApplicationUser User { get; set; } = default!;
}