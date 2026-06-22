using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Addresses.Dtos;
using Dukaan.Application.Features.Customers.Queries.GetCurrentCustomerId;
using Dukaan.Application.Interfaces;
using Dukaan.Domain.Entities;
using ErrorOr;
using MediatR;

namespace Dukaan.Application.Features.Addresses.Queries.GetAddressById;

public class GetAddressByIdHandler(
    IRepository<Address> repository,
    IMediator mediator)
    : IQueryHandler<GetAddressByIdQuery, ErrorOr<AddressDto>>
{
    public async Task<ErrorOr<AddressDto>> Handle(GetAddressByIdQuery request, CancellationToken cancellationToken)
    {
        var customerIdResult = await mediator.Send(new GetCurrentCustomerIdQuery(), cancellationToken);
        if (customerIdResult.IsError) return AddressErrors.CustomerNotFound;
        
        var customerId = customerIdResult.Value;
        if (customerId is null) return AddressErrors.CustomerNotFound;

        var address = await repository.GetByIdAsync(request.Id, trackChanges: false, cancellationToken: cancellationToken);
        if (address is null) return AddressErrors.NotFound;
        
        if (address.CustomerId != customerId) return AddressErrors.NotOwned;

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
