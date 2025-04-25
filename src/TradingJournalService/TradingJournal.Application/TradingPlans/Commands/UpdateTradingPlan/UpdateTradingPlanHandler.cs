using TradingJournal.Application.Repositories;

namespace TradingJournal.Application.TradingPlans.Commands.UpdateTradingPlan;

public class UpdateTradingPlanHandler(ITradingPlanRepository repo)
    : ICommandHandler<UpdateTradingPlanCommand, UpdateTradingPlanResult>
{
    public async Task<UpdateTradingPlanResult> Handle(UpdateTradingPlanCommand command,
        CancellationToken cancellationToken)
    {
        var tradingPlanId = TradingPlanId.Of(command.Id);

        var tradingPlan = await repo.GetByIdAsync(tradingPlanId, cancellationToken);

        if (tradingPlan is null)
        {
            throw new NotFoundException(ExceptionMessages.NotFound(nameof(TradingPlan),
                nameof(command.Id), command.Id.ToString()));
        }

        await UpdateTradingPlanWithNewValues(tradingPlan, command, cancellationToken);

        var status = await repo.UpdateAsync(tradingPlan, cancellationToken);

        if (status == false)
        {
            throw new InternalServerException(ExceptionMessages.InternalServerForUpdate(nameof(TradingPlan),
                nameof(tradingPlan.Name), tradingPlan.Name));
        }

        return new UpdateTradingPlanResult
        {
            IsSuccess = true
        };
    }

    private async Task UpdateTradingPlanWithNewValues(TradingPlan tradingPlan,
        UpdateTradingPlanCommand updateTradingPlanCommand, CancellationToken cancellationToken)
    {
        tradingPlan.Update(updateTradingPlanCommand.Name, updateTradingPlanCommand.FromTime, updateTradingPlanCommand.ToTime,
            updateTradingPlanCommand.SelectedDays);

        var technics = await repo.GetTechnicsByIdsAsync(updateTradingPlanCommand.Technics, cancellationToken);
        
        foreach (var technic in technics)
        {
            tradingPlan.UpdateTechnic(technic);
        }

        //Order is important; this line of code must come after the UpdateTechnic code.
        foreach (var id in updateTradingPlanCommand.RemovedTechnics)
        {
            tradingPlan.RemoveTechnic(TradingTechnicId.Of(id));
        }
    }
}