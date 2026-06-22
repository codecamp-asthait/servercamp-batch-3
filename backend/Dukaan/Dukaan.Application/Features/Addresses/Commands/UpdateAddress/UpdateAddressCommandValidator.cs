using Dukaan.Application.Features.Addresses.Dtos;
using FluentValidation;

namespace Dukaan.Application.Features.Addresses.Commands.UpdateAddress;

public class UpdateAddressDataValidator : AbstractValidator<UpdateAddressData>
{
    public UpdateAddressDataValidator()
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

public class UpdateAddressCommandValidator : AbstractValidator<UpdateAddressCommand>
{
    public UpdateAddressCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Address ID is required.");
        RuleFor(x => x.Data).SetValidator(new UpdateAddressDataValidator());
    }
}
