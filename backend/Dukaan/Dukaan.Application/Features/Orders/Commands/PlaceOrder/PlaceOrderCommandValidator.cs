using FluentValidation;

namespace Dukaan.Application.Features.Orders.Commands.PlaceOrder;

public class PlaceOrderCommandValidator : AbstractValidator<PlaceOrderCommand>
{
    public PlaceOrderCommandValidator()
    {
        RuleFor(x => x.BillingAddressId)
            .NotEmpty().WithMessage("Billing address is required.");
        RuleFor(x => x.DeliveryAddressId)
            .NotEmpty().WithMessage("Delivery address is required.");
    }
}
