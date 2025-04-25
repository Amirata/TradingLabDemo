using Contracts.User;
using MassTransit;
using TradingJournal.Application.Users.Commands.UpdateUser;

namespace TradingJournal.Api.Consumers;

public class UserUpdatedConsumer(ISender sender) : IConsumer<UserUpdated>
{
    public async Task Consume(ConsumeContext<UserUpdated> context)
    {
        await sender.Send(new UpdateUserCommand
        {
            Id = context.Message.Id,
            UserName = context.Message.UserName,
        });
    }
}
