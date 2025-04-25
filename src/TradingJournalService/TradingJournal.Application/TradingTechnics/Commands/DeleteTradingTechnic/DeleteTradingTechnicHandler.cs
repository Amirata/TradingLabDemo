using BuildingBlocks.Utilities;
using TradingJournal.Application.Repositories;

namespace TradingJournal.Application.TradingTechnics.Commands.DeleteTradingTechnic;

public class DeleteTradingTechnicHandler(ITradingTechnicRepository repo)
    : ICommandHandler<DeleteTradingTechnicCommand, DeleteTradingTechnicResult>
{
    public async Task<DeleteTradingTechnicResult> Handle(DeleteTradingTechnicCommand command,
        CancellationToken cancellationToken)
    {
        var imageUpload = new ImageUpload("TechnicImages");
        var tradingTechnicId = TradingTechnicId.Of(command.TradingTechnicId);
        var tradingTechnic = await repo.GetByIdAsync(tradingTechnicId, cancellationToken: cancellationToken);

        if (tradingTechnic is null)
        {
            throw new NotFoundException(ExceptionMessages.NotFound(nameof(TradingTechnic),
                nameof(command.TradingTechnicId),
                command.TradingTechnicId.ToString()));
        }

        var status = await repo.DeleteAsync(tradingTechnic, cancellationToken);
        if (status == false)
        {
            throw new InternalServerException(ExceptionMessages.InternalServerForDelete(nameof(TradingTechnic),
                nameof(tradingTechnic.Name), tradingTechnic.Name));
        }
        
        foreach (var image in tradingTechnic.Images)
        {
            imageUpload.RemoveImage(image.Path);
        }


        return new DeleteTradingTechnicResult
        {
            IsSuccess = true
        };
    }
}