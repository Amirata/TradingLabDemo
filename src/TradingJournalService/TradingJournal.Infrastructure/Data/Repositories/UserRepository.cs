using AutoMapper.QueryableExtensions;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Pagination;
using BuildingBlocks.Utilities;
using Npgsql;
using TradingJournal.Application.Trades.Queries.GetTradeById;
using TradingJournal.Application.Trades.Queries.GetTrades;

namespace TradingJournal.Infrastructure.Data.Repositories;

public class UserRepository(ApplicationDbContext context) : IUserRepository
{
    public async Task<bool> CreateAsync(User user, CancellationToken cancellationToken)
    {
        context.Users.Add(user);
        return await context.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<User?> GetByIdAsync(UserId userId, CancellationToken cancellationToken)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(t => t.Id == userId, cancellationToken: cancellationToken);

        return user;
    }

    public async Task<bool> DeleteAsync(User user, CancellationToken cancellationToken)
    {
        context.Users.Remove(user);
        return await context.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> UpdateAsync(User user, CancellationToken cancellationToken)
    {
        context.Users.Update(user);
        return await context.SaveChangesAsync(cancellationToken) > 0;
    }
}