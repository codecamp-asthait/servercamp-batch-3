using MediatR;

namespace Dukaan.Media.Application.Core.Abstractions;

public interface ICommand<TResponse> : IRequest<TResponse> { }
public interface ICommand : IRequest { }
