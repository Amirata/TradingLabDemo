using TradingJournal.Application.Repositories;

namespace TradingJournal.Application.TradingPlans.Commands.DeleteTradingPlan;
public class DeleteTradingPlanHandler(ITradingPlanRepository repo)
    : ICommandHandler<DeleteTradingPlanCommand, DeleteTradingPlanResult>
{
    public async Task<DeleteTradingPlanResult> Handle(DeleteTradingPlanCommand command, CancellationToken cancellationToken)
    {
        var tradingPlanId = TradingPlanId.Of(command.TradingPlanId);
        
        
        var tradingPlan =await repo.GetByIdAsync(tradingPlanId, cancellationToken: cancellationToken);

        if (tradingPlan is null)
        {
            throw new NotFoundException(ExceptionMessages.NotFound(nameof(TradingPlan),nameof(command.TradingPlanId),command.TradingPlanId.ToString()));
        }

        var status = await repo.DeleteAsync(tradingPlan, cancellationToken);
        
        if (status == false)
        {
            throw new InternalServerException(ExceptionMessages.InternalServerForDelete(nameof(TradingPlan),nameof(tradingPlan.Name),tradingPlan.Name));
        }
       

        return new DeleteTradingPlanResult
        {
            IsSuccess = true
        };        
    }
}
