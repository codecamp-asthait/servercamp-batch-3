using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Addresses.Dtos;
using Dukaan.Application.Features.Customers.Queries.GetCurrentCustomerId;
using Dukaan.Application.Interfaces;
using Dukaan.Domain.Entities;
using ErrorOr;
using MediatR;

namespace Dukaan.Application.Features.Addresses.Commands.UpdateAddress;

public class UpdateAddressHandler(
    IRepository<Address> repository,
    IMediator mediator)
    : ICommandHandler<UpdateAddressCommand, ErrorOr<AddressDto>>
{
    public async Task<ErrorOr<AddressDto>> Handle(UpdateAddressCommand request, CancellationToken cancellationToken)
    {
        var customerIdResult = await mediator.Send(new GetCurrentCustomerIdQuery(), cancellationToken);
        if (customerIdResult.IsError) return AddressErrors.CustomerNotFound;

        var customerId = customerIdResult.Value;
        if (customerId is null) return AddressErrors.CustomerNotFound;

        var address = await repository.GetByIdAsync(request.Id, trackChanges: true, cancellationToken: cancellationToken);
        if (address is null) return AddressErrors.NotFound;

        if (address.CustomerId != customerId) return AddressErrors.NotOwned;

        address.Label = request.Data.Label;
        address.Street = request.Data.Street;
        address.City = request.Data.City;
        address.District = request.Data.District;
        address.PostalCode = request.Data.PostalCode;
        address.Phone = request.Data.Phone;

        repository.Update(address);
        await repository.SaveChangesAsync(cancellationToken);

        return MapToDto(address);
    }

    private static AddressDto MapToDto(Address address)
    {
        return new AddressDto(
            address.Id,
            address.Label,
            address.Type,
            address.Street,
            address.City,
            address.District,
            address.PostalCode,
            address.Phone,
            address.IsDefault
        );
    }
}
