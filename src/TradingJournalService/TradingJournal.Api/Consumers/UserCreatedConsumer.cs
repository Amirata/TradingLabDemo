using MassTransit;
using Contracts.User;
using TradingJournal.Application.Users.Commands.CreateUser;

namespace TradingJournal.Api.Consumers;

public class UserCreatedConsumer(ISender sender) : IConsumer<UserCreated>
{
    public async Task Consume(ConsumeContext<UserCreated> context)
    {
        await sender.Send(new CreateUserCommand
        {
            Id = context.Message.Id,
            UserName = context.Message.UserName,
        });
    }
}