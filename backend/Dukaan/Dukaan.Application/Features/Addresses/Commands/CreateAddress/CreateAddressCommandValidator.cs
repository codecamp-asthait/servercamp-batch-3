using FluentValidation;

namespace Dukaan.Application.Features.Addresses.Commands.CreateAddress;

public class CreateAddressCommandValidator : AbstractValidator<CreateAddressCommand>
{
    public CreateAddressCommandValidator()
    {
        RuleFor(x => x.Label).NotEmpty().WithMessage("Label is required.");
        RuleFor(x => x.Street).NotEmpty().WithMessage("Street is required.");
        RuleFor(x => x.City).NotEmpty().WithMessage("City is required.");
        RuleFor(x => x.District).NotEmpty().WithMessage("District is required.");
        RuleFor(x => x.PostalCode).NotEmpty().WithMessage("Postal code is required.");
        RuleFor(x => x.Phone).NotEmpty().WithMessage("Phone is required.");
        RuleFor(x => x.Phone).Matches(@"^[\d\+\-\s()]+$").WithMessage("Phone must be a valid format.");
    }
}
