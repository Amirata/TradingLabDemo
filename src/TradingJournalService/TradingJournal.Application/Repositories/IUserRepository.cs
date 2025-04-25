namespace TradingJournal.Application.Repositories;

public interface IUserRepository
{ 
    Task<bool> CreateAsync(User user, CancellationToken cancellationToken);
    Task<User?> GetByIdAsync(UserId userId, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(User user, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(User user, CancellationToken cancellationToken);
}