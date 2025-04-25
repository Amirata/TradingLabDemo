namespace Contracts.User;

public class UserCreated
{
    public Guid Id { get; set; }
    public required string UserName { get; set; }
}