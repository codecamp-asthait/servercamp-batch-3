using FluentValidation;

namespace Dukaan.Application.Features.Addresses.Commands.UpdateAddress;

public class UpdateAddressCommandValidator : AbstractValidator<UpdateAddressCommand>
{
    public UpdateAddressCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Address ID is required.");
        RuleFor(x => x.Data).SetValidator(new UpdateAddressDataValidator());
    }
}
