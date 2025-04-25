using TradingJournal.Application.TradingTechnics.Queries.GetTradingTechnicById;
using TradingJournal.Application.TradingTechnics.Queries.GetTradingTechnicByName;
using TradingJournal.Application.TradingTechnics.Queries.GetTradingTechnics;

namespace TradingJournal.Infrastructure.Mappings;

public class TradingTechnicMapping : Profile
{
    public TradingTechnicMapping()
    {
        CreateMap<TradingTechnic, GetTradingTechnicByIdResult>()
            .ForMember(d => d.Id, o => o.MapFrom(src => src.Id.Value));

        CreateMap<TradingTechnic, GetTradingTechnicByNameResult>()
            .ForMember(d => d.Id, o => o.MapFrom(src => src.Id.Value));

        CreateMap<TradingTechnic, GetTradingTechnicsResult>()
            .ForMember(d => d.Id, o => o.MapFrom(src => src.Id.Value));
    }
}