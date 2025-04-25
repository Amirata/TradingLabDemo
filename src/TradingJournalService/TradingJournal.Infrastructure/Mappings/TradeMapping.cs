using TradingJournal.Application.Trades.Queries.GetTradeById;
using TradingJournal.Application.Trades.Queries.GetTrades;

namespace TradingJournal.Infrastructure.Mappings;

public class TradeMapping : Profile
{
    public TradeMapping()
    {
        CreateMap<Trade, GetTradeByIdResult>()
            .ForMember(d => d.Id, o => o.MapFrom(src => src.Id.Value))
            .ForMember(d => d.TradingPlanId, o => o.MapFrom(src => src.TradingPlanId.Value));
        
        CreateMap<Trade, GetTradesResult>()
            .ForMember(d => d.Id, o => o.MapFrom(src => src.Id.Value))
            .ForMember(d => d.TradingPlanId, o => o.MapFrom(src => src.TradingPlanId.Value));
    }
    
}