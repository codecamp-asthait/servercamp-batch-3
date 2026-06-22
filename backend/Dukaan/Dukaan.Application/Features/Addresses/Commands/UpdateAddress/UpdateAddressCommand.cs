using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Addresses.Dtos;
using ErrorOr;

namespace Dukaan.Application.Features.Addresses.Commands.UpdateAddress;

public record UpdateAddressCommand(Guid Id, UpdateAddressData Data)
    : ICommand<ErrorOr<AddressDto>>;
