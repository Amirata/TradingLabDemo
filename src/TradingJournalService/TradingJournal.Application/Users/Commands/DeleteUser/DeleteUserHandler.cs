using TradingJournal.Application.Repositories;

namespace TradingJournal.Application.Users.Commands.DeleteUser;
public class DeleteUserHandler(IUserRepository repo)
    : ICommandHandler<DeleteUserCommand, DeleteUserResult>
{
    public async Task<DeleteUserResult> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        var userId = UserId.Of(command.Id);
        
        
        var user = await repo.GetByIdAsync(userId, cancellationToken: cancellationToken);

        if (user is null)
        {
            throw new NotFoundException(ExceptionMessages.NotFound(nameof(User),nameof(command.Id),command.Id.ToString()));
        }

        var status = await repo.DeleteAsync(user, cancellationToken);
        
        if (status == false)
        {
            throw new InternalServerException(ExceptionMessages.InternalServerForDelete(nameof(User),nameof(user.Name),user.Name));
        }
       

        return new DeleteUserResult
        {
            IsSuccess = true
        };        
    }
}
