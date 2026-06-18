using Dukaan.Media.Application.Core.Abstractions;
using ErrorOr;

namespace Dukaan.Media.Application.Features.Media.Commands.DeleteMedia;

public record DeleteMediaCommand(Guid MediaId) : ICommand<ErrorOr<Deleted>>;
