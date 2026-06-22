using Dukaan.Application.Core.Abstractions;
using Dukaan.Application.Features.Addresses.Dtos;
using ErrorOr;

namespace Dukaan.Application.Features.Addresses.Commands.SetDefaultAddress;

public record SetDefaultAddressCommand(Guid Id) : ICommand<ErrorOr<AddressDto>>;
