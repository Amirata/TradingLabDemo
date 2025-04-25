using BuildingBlocks.Utilities;
using TradingJournal.Application.Repositories;

namespace TradingJournal.Application.TradingTechnics.Commands.UpdateTradingTechnic;

public class UpdateTradingTechnicHandler(ITradingTechnicRepository repo)
    : ICommandHandler<UpdateTradingTechnicCommand, UpdateTradingTechnicResult>
{
    public async Task<UpdateTradingTechnicResult> Handle(UpdateTradingTechnicCommand command,
        CancellationToken cancellationToken)
    {
        var tradingTechnicId = TradingTechnicId.Of(command.Id);
        var tradingTechnic = await repo.GetByIdAsync(tradingTechnicId, cancellationToken);

        if (tradingTechnic is null)
        {
            throw new NotFoundException(ExceptionMessages.NotFound(nameof(TradingTechnic),
                nameof(command.Id),
                command.Id.ToString()));
        }

        await UpdateTradingTechnicWithNewValues(tradingTechnic, command);

        var status = await repo.UpdateAsync(tradingTechnic, cancellationToken);

        if (status == false)
        {
            throw new InternalServerException(ExceptionMessages.InternalServerForUpdate(nameof(TradingTechnic),
                nameof(tradingTechnic.Name), tradingTechnic.Name));
        }

        return new UpdateTradingTechnicResult
        {
            IsSuccess = true,
        };
    }

    private async Task UpdateTradingTechnicWithNewValues(TradingTechnic tradingTechnic, UpdateTradingTechnicCommand updateTradingTechnicCommand)
    {
        var images = new List<string>();
        
        var imageUpload = new ImageUpload("TechnicImages");
        imageUpload.CreateDirectory();
        
        foreach (var image in updateTradingTechnicCommand.NewImages)
        {
            var imagePath = await imageUpload.SaveImageAsync(image);
            images.Add(imagePath);
        }
        
        foreach (var path in updateTradingTechnicCommand.RemovedImages)
        {
            imageUpload.RemoveImage(path);
        }
        
        tradingTechnic.Update(
            updateTradingTechnicCommand.Name,
            updateTradingTechnicCommand.Description);
        images.AddRange(updateTradingTechnicCommand.Images);

        foreach (var path in images)
        {
            tradingTechnic.UpdateImage(path);
        }

        foreach (var path in updateTradingTechnicCommand.RemovedImages)
        {
            tradingTechnic.RemoveImage(path);
        }
    }
}