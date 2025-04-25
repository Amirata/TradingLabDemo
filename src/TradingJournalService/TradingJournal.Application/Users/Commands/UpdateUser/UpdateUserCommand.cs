using FluentValidation;

namespace TradingJournal.Application.Users.Commands.UpdateUser;

public class UpdateUserCommand
    : ICommand<UpdateUserResult>
{
    public Guid Id { get; set; } = default!;
    
    public string UserName { get; set; } = default!;
}

public class UpdateUserResult
{
    public bool IsSuccess { get; set; }
}


