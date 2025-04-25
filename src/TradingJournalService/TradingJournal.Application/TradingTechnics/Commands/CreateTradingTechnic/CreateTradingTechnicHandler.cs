using BuildingBlocks.Utilities;
using TradingJournal.Application.Repositories;

namespace TradingJournal.Application.TradingTechnics.Commands.CreateTradingTechnic;

public class CreateTradingTechnicHandler(ITradingTechnicRepository repo)
    : ICommandHandler<CreateTradingTechnicCommand, CreateTradingTechnicResult>
{
    public async Task<CreateTradingTechnicResult> Handle(CreateTradingTechnicCommand command,
        CancellationToken cancellationToken)
    {
        var tradingTechnic = await CreateNewTradingTechnic(command);

        var status = await repo.CreateAsync(tradingTechnic, cancellationToken);

        if (status == false)
        {
            throw new InternalServerException(ExceptionMessages.InternalServerForCreate(nameof(TradingTechnic),nameof(tradingTechnic.Name),tradingTechnic.Name));
        }

        return new CreateTradingTechnicResult
        {
            Id = tradingTechnic.Id.Value
        };
    }

    private async Task<TradingTechnic> CreateNewTradingTechnic(CreateTradingTechnicCommand createTradingTechnicCommand)
    {
        var images = new List<string>();
        
        var imageUpload = new ImageUpload("TechnicImages");
        imageUpload.CreateDirectory();
        
        foreach (var image in createTradingTechnicCommand.NewImages)
        {
            var imagePath = await imageUpload.SaveImageAsync(image);
            images.Add(imagePath);
            
        }
        
        var newTradingTechnic = TradingTechnic.Create(
            TradingTechnicId.New(),
            createTradingTechnicCommand.Name,
            createTradingTechnicCommand.Description,
            UserId.Of(createTradingTechnicCommand.UserId)
        );

        foreach (var path in images)
        {
            newTradingTechnic.AddImage(path);
        }

        return newTradingTechnic;
    }
}