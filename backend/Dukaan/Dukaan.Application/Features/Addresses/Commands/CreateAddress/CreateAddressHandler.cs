using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Addresses.Dtos;
using Dukaan.Application.Features.Customers.Queries.GetCurrentCustomerId;
using Dukaan.Application.Interfaces;
using Dukaan.Domain.Entities;
using ErrorOr;
using MediatR;

namespace Dukaan.Application.Features.Addresses.Commands.CreateAddress;

public class CreateAddressHandler(
    IRepository<Address> repository,
    IMediator mediator)
    : ICommandHandler<CreateAddressCommand, ErrorOr<AddressDto>>
{
    public async Task<ErrorOr<AddressDto>> Handle(CreateAddressCommand request, CancellationToken cancellationToken)
    {
        var customerIdResult = await mediator.Send(new GetCurrentCustomerIdQuery(), cancellationToken);
        if (customerIdResult.IsError) return AddressErrors.CustomerNotFound;

        var customerId = customerIdResult.Value;
        if (customerId is null) return AddressErrors.CustomerNotFound;

        if (request.IsDefault)
        {
            var existingDefaults = await repository.FindAsync(
                a => a.CustomerId == customerId && a.Type == request.Type && a.IsDefault,
                trackChanges: true,
                cancellationToken);

            foreach (var existing in existingDefaults)
            {
                existing.IsDefault = false;
                repository.Update(existing);
            }
        }

        var address = new Address
        {
            CustomerId = customerId.Value,
            Label = request.Label,
            Type = request.Type,
            Street = request.Street,
            City = request.City,
            District = request.District,
            PostalCode = request.PostalCode,
            Phone = request.Phone,
            IsDefault = request.IsDefault
        };

        await repository.AddAsync(address, cancellationToken);
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
