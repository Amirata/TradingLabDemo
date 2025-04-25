namespace TradingJournal.Application.Users.Commands.DeleteUser;

public class DeleteUserCommand
    : ICommand<DeleteUserResult>
{
    public Guid Id { get; set; }
}

public class DeleteUserResult
{
    public bool IsSuccess { get; set; }
}

