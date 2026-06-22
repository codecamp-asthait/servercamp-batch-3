using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Addresses.Dtos;
using Dukaan.Application.Features.Customers.Queries.GetCurrentCustomerId;
using Dukaan.Application.Interfaces;
using Dukaan.Domain.Entities;
using ErrorOr;
using MediatR;

namespace Dukaan.Application.Features.Addresses.Queries.GetCustomerAddresses;

public class GetCustomerAddressesHandler(
    IRepository<Address> repository,
    IMediator mediator)
    : IQueryHandler<GetCustomerAddressesQuery, ErrorOr<List<AddressDto>>>
{
    public async Task<ErrorOr<List<AddressDto>>> Handle(GetCustomerAddressesQuery request, CancellationToken cancellationToken)
    {
        var customerIdResult = await mediator.Send(new GetCurrentCustomerIdQuery(), cancellationToken);
        if (customerIdResult.IsError) return AddressErrors.CustomerNotFound;
        
        var customerId = customerIdResult.Value;
        if (customerId is null) return AddressErrors.CustomerNotFound;

        var addresses = await repository.FindAsync(
            a => a.CustomerId == customerId,
            trackChanges: false,
            cancellationToken);

        return addresses.Select(MapToDto).ToList();
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
