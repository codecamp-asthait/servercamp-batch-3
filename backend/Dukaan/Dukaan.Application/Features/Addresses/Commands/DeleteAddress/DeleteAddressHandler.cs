using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Customers.Queries.GetCurrentCustomerId;
using Dukaan.Application.Interfaces;
using Dukaan.Domain.Entities;
using ErrorOr;
using MediatR;

namespace Dukaan.Application.Features.Addresses.Commands.DeleteAddress;

public class DeleteAddressHandler(
    IRepository<Address> repository,
    IMediator mediator)
    : ICommandHandler<DeleteAddressCommand, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(DeleteAddressCommand request, CancellationToken cancellationToken)
    {
        var customerIdResult = await mediator.Send(new GetCurrentCustomerIdQuery(), cancellationToken);
        if (customerIdResult.IsError) return AddressErrors.CustomerNotFound;
        
        var customerId = customerIdResult.Value;
        if (customerId is null) return AddressErrors.CustomerNotFound;

        var address = await repository.GetByIdAsync(request.Id, trackChanges: false, cancellationToken: cancellationToken);
        if (address is null) return AddressErrors.NotFound;
        
        if (address.CustomerId != customerId) return AddressErrors.NotOwned;
        
        if (address.IsDefault) return AddressErrors.CannotDeleteDefault;

        repository.Remove(address);
        await repository.SaveChangesAsync(cancellationToken);

        return Result.Deleted;
    }
}
