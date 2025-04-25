namespace Contracts.User;

public class UserUpdated
{
    public Guid Id { get; set; }
    public required string UserName { get; set; }
}