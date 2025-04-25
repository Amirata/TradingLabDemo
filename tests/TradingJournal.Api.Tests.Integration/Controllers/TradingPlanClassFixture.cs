using TradingJournal.Application.TradingPlans.Commands.CreateTradingPlan;
using TradingJournal.Application.TradingPlans.Commands.UpdateTradingPlan;
using TradingJournal.Application.TradingPlans.Queries.GetTradingPlans;
using TradingJournal.Application.TradingTechnics.Queries.GetTradingTechnicById;

namespace TradingJournal.Api.Tests.Integration.Controllers;

public class TradingPlanClassFixture
{
    
    internal string Uri = "api/v1/trading-plan";
    internal string TradingTechnicUri = "api/v1/trading-technic";
    
    internal Guid Id = Guid.Empty;
    internal List<GetTradingTechnicByIdResult> TradingTechnics = new();
    internal List<GetTradingPlansResult> GetTradingPlansResultList = new();
    internal CreateTradingPlanCommand? CreateTradingPlanCommand;
    internal UpdateTradingPlanCommand? UpdateTradingPlanCommand;
    internal readonly string UserId = "E671C0F9-B5AF-4D75-B2D2-32C00A31A494";
    internal readonly string UserName = "Admin";
}