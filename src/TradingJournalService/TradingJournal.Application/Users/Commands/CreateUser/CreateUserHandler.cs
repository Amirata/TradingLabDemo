using TradingJournal.Application.Repositories;

namespace TradingJournal.Application.Users.Commands.CreateUser;

public class CreateUserHandler(IUserRepository repo)
    : ICommandHandler<CreateUserCommand, CreateUserResult>
{
    public async Task<CreateUserResult> Handle(CreateUserCommand command,
        CancellationToken cancellationToken)
    {
        var user = User.Create(
            UserId.Of(command.Id),
            command.UserName
        );

        var status = await repo.CreateAsync(user, cancellationToken);
        
        if (status == false)
        {
            throw new InternalServerException(ExceptionMessages.InternalServerForCreate(nameof(User),nameof(user.Name),user.Name));
        }

        return new CreateUserResult { IsSuccessful = true };
    }
}