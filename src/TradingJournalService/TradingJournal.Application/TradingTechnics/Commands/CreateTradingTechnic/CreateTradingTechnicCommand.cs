using System.Collections;
using FluentValidation;
using Microsoft.AspNetCore.Http;


namespace TradingJournal.Application.TradingTechnics.Commands.CreateTradingTechnic;

public record CreateTradingTechnicCommand
    : ICommand<CreateTradingTechnicResult>
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public ICollection<IFormFile> NewImages { get; set; } = [];

    public Guid UserId { get; set; }
}

public class CreateTradingTechnicResult
{
    public Guid Id { get; set; }
};

public class CreateTradingTechnicCommandValidator : AbstractValidator<CreateTradingTechnicCommand>
{
    public CreateTradingTechnicCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
        RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required.");
        RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required.");
    }
}