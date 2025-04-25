using TradingJournal.Application.TradingPlans.Queries.GetTradingPlanById;
using TradingJournal.Application.TradingPlans.Queries.GetTradingPlanByName;
using TradingJournal.Application.TradingPlans.Queries.GetTradingPlans;
using TradingJournal.Domain.Enums;

namespace TradingJournal.Infrastructure.Mappings;

public class TradingPlanMapping : Profile
{
    public TradingPlanMapping()
    {
        
        CreateMap<TradingPlan, GetTradingPlanByIdResult>()
            .ForMember(d => d.Id, o => o.MapFrom(src=>src.Id.Value))
            .ForMember(d => d.SelectedDays, o => o.MapFrom(src=>ConvertDaysOfWeekToList(src.SelectedDays)));
        
        CreateMap<TradingPlan, GetTradingPlanByNameResult>()
            .ForMember(d => d.Id, o => o.MapFrom(src=>src.Id.Value))
            .ForMember(d => d.SelectedDays, o => o.MapFrom(src=>ConvertDaysOfWeekToList(src.SelectedDays)));

        CreateMap<TradingPlan, GetTradingPlansResult>()
            .ForMember(d => d.Id, o => o.MapFrom(src=>src.Id.Value))
            .ForMember(d => d.SelectedDays, o => o.MapFrom(src=>ConvertDaysOfWeekToList(src.SelectedDays)));

    }

    private static List<string> ConvertDaysOfWeekToList(DaysOfWeek daysOfWeek)
    {
        var daysOfWeekList = new List<string>();
        if ((daysOfWeek & DaysOfWeek.Sunday) == DaysOfWeek.Sunday)
        {
            daysOfWeekList.Add(DayOfWeek.Sunday.ToString());
        }
        if ((daysOfWeek & DaysOfWeek.Monday) == DaysOfWeek.Monday)
        {
            daysOfWeekList.Add(DayOfWeek.Monday.ToString());
        }
        if ((daysOfWeek & DaysOfWeek.Tuesday) == DaysOfWeek.Tuesday)
        {
            daysOfWeekList.Add(DayOfWeek.Tuesday.ToString());
        }
        if ((daysOfWeek & DaysOfWeek.Wednesday) == DaysOfWeek.Wednesday)
        {
            daysOfWeekList.Add(DayOfWeek.Wednesday.ToString());
        }
        if ((daysOfWeek & DaysOfWeek.Thursday) == DaysOfWeek.Thursday)
        {
            daysOfWeekList.Add(DayOfWeek.Thursday.ToString());
        }
        if ((daysOfWeek & DaysOfWeek.Friday) == DaysOfWeek.Friday)
        {
            daysOfWeekList.Add(DayOfWeek.Friday.ToString());
        }
        if ((daysOfWeek & DaysOfWeek.Saturday) == DaysOfWeek.Saturday)
        {
            daysOfWeekList.Add(DayOfWeek.Saturday.ToString());
        }

        return daysOfWeekList;
    }
    
}