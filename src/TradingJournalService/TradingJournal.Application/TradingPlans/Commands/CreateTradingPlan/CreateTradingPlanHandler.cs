using TradingJournal.Application.Repositories;

namespace TradingJournal.Application.TradingPlans.Commands.CreateTradingPlan;

public class CreateTradingPlanHandler(ITradingPlanRepository repo)
    : ICommandHandler<CreateTradingPlanCommand, CreateTradingPlanResult>
{
    public async Task<CreateTradingPlanResult> Handle(CreateTradingPlanCommand command,
        CancellationToken cancellationToken)
    {
        var tradingPlan = await CreateNewTradingPlan(command, cancellationToken);

        var status = await repo.CreateAsync(tradingPlan, cancellationToken);
        
        if (status == false)
        {
            throw new InternalServerException(ExceptionMessages.InternalServerForCreate(nameof(TradingPlan),nameof(tradingPlan.Name),tradingPlan.Name));
        }

        return new CreateTradingPlanResult { Id = tradingPlan.Id.Value };
    }

    private async Task<TradingPlan> CreateNewTradingPlan(CreateTradingPlanCommand createTradingPlanCommand,
        CancellationToken cancellationToken)
    {
        var newTradingPlan = TradingPlan.Create(
            TradingPlanId.New(),
            createTradingPlanCommand.Name,
            createTradingPlanCommand.FromTime,
            createTradingPlanCommand.ToTime,
            createTradingPlanCommand.SelectedDays,
            UserId.Of(createTradingPlanCommand.UserId)
        );


        var technics = await repo.GetTechnicsByIdsAsync(createTradingPlanCommand.Technics, cancellationToken);
        
        
        if (technics.Count == 0)
        {
            throw new BadRequestException(ExceptionMessages.BadRequest(nameof(TradingTechnicId)));
        }

        foreach (var technic in technics)
        {
            newTradingPlan.AddTechnic(technic);
        }

        return newTradingPlan;
    }
}