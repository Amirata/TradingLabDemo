using Contracts.User;
using MassTransit;
using TradingJournal.Application.Users.Commands.DeleteUser;

namespace TradingJournal.Api.Consumers;

public class UserDeletedConsumer(ISender sender) : IConsumer<UserDeleted>
{
    public async Task Consume(ConsumeContext<UserDeleted> context)
    {
        await sender.Send(new DeleteUserCommand
        {
            Id = context.Message.Id
        });
    }
}
