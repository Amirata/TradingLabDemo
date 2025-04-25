using TradingJournal.Application.TradingTechnics.Commands.CreateTradingTechnic;
using TradingJournal.Application.TradingTechnics.Commands.UpdateTradingTechnic;
using TradingJournal.Application.TradingTechnics.Queries.GetTradingTechnics;

namespace TradingJournal.Api.Tests.Integration.Controllers;

public class TradingTechnicClassFixture
{
    
    internal string Uri = "api/v1/trading-technic";
    
    internal Guid Id = Guid.Empty;
    internal CreateTradingTechnicCommand? CreateTradingTechnicCommand;
    internal UpdateTradingTechnicCommand? UpdateTradingTechnicCommand;
    internal List<GetTradingTechnicsResult> GetTradingTechnicsResultList = new();
    internal readonly string UserId = "E671C0F9-B5AF-4D75-B2D2-32C00A31A494";
    internal readonly string UserName = "Admin";
}