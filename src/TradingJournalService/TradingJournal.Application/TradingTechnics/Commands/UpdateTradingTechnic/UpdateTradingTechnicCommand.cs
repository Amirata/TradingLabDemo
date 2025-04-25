using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace TradingJournal.Application.TradingTechnics.Commands.UpdateTradingTechnic;

public class UpdateTradingTechnicCommand
    : ICommand<UpdateTradingTechnicResult>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public ICollection<IFormFile> NewImages { get; set; } = [];
    public ICollection<string> Images { get; set; } = [];
    public ICollection<string> RemovedImages { get; set; } = [];
}

public class UpdateTradingTechnicResult
{
    public bool IsSuccess { get; set; }
}

public class UpdateTradingTechnicCommandValidator : AbstractValidator<UpdateTradingTechnicCommand>
{
    public UpdateTradingTechnicCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required.");
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
        RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required.");
        RuleFor(x => x).Must(t=>!t.Images.Any(r=>t.RemovedImages.Contains(r))).WithMessage("The Images contains values that are also present in the RemovedImages.");

    }
}

