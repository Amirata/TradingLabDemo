using AutoMapper;
using TradingJournal.Infrastructure.Mappings;

namespace TradingJournal.Infrastructure.Tests.Unit;

public class MappingTest
{
    private readonly IMapper _mapper;


    public MappingTest()
    {
        var config = new MapperConfiguration(
            cfg =>
            {
                cfg.AddProfile(new TradingTechnicMapping());
                cfg.AddProfile(new TradingPlanMapping());
                cfg.AddProfile(new TradingTechnicImageMapping());
                cfg.AddProfile(new TradeMapping());
            });

        _mapper = new Mapper(config);
    }

    [Fact]
    public void Mapping_AllNeededPropertyMap_WhenMapping()
    {
        _mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }
}