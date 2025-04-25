using FluentValidation;

namespace TradingJournal.Application.Users.Commands.CreateUser;

public class CreateUserCommand : ICommand<CreateUserResult>
{
    public Guid Id { get; set; } = default!;
    
    public string UserName { get; set; } = default!;
}

public class CreateUserResult
{
    public bool IsSuccessful { get; set; }   
}