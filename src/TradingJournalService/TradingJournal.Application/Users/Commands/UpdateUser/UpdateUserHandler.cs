using TradingJournal.Application.Repositories;

namespace TradingJournal.Application.Users.Commands.UpdateUser;

public class UpdateUserHandler(IUserRepository repo)
    : ICommandHandler<UpdateUserCommand, UpdateUserResult>
{
    public async Task<UpdateUserResult> Handle(UpdateUserCommand command,
        CancellationToken cancellationToken)
    {
        var userId = UserId.Of(command.Id);

        var user = await repo.GetByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            throw new NotFoundException(ExceptionMessages.NotFound(nameof(User),
                nameof(command.Id), command.Id.ToString()));
        }
        
        user.Update(command.UserName);

        var status = await repo.UpdateAsync(user, cancellationToken);

        if (status == false)
        {
            throw new InternalServerException(ExceptionMessages.InternalServerForUpdate(nameof(User),
                nameof(user.Name), user.Name));
        }

        return new UpdateUserResult
        {
            IsSuccess = true
        };
    }
}