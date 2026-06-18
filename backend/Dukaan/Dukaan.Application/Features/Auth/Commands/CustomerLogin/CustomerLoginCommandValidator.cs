using FluentValidation;

namespace Dukaan.Application.Features.Auth.Commands.CustomerLogin;

public class CustomerLoginCommandValidator : AbstractValidator<CustomerLoginCommand>
{
    public CustomerLoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email must be a valid email address.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}
