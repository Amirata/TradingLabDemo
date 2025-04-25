namespace TradingJournal.Infrastructure.Mappings;

public class TradingTechnicImageMapping : Profile
{
    public TradingTechnicImageMapping()
    {
        CreateMap<TradingTechnicImage, string>()
            .ConvertUsing(t => t.Path);
    }
}