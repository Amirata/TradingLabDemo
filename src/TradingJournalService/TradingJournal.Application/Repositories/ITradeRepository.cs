using BuildingBlocks.Pagination;
using TradingJournal.Application.Trades.Queries.GetTradeById;
using TradingJournal.Application.Trades.Queries.GetTrades;


namespace TradingJournal.Application.Repositories;
public interface ITradeRepository
{
   Task<bool> CreateAsync(Trade trade, CancellationToken cancellationToken);
   
   Task<Trade?> GetByIdAsync(TradeId id, CancellationToken cancellationToken);
   
   Task<bool> DeleteAsync(Trade trade, CancellationToken cancellationToken);
   
   Task<bool> UpdateAsync(Trade trade, CancellationToken cancellationToken);

   Task<GetTradeByIdResult?> GetTradeByIdAsync(Guid id, CancellationToken cancellationToken);
 
   Task<int> CountAsync(string? search, Guid planId, CancellationToken cancellationToken);
   
   Task<PaginatedResult<GetTradesResult>> GetTradesAsync(GetTradesQuery paginationRequest,CancellationToken cancellationToken);

   Task<bool> TradingPlanExistAsync(Guid id, CancellationToken cancellationToken);

}
