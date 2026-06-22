using Dukaan.Application.Core.Abstractions;
using ErrorOr;

namespace Dukaan.Application.Features.Addresses.Commands.DeleteAddress;

public record DeleteAddressCommand(Guid Id) : ICommand<ErrorOr<Deleted>>;
