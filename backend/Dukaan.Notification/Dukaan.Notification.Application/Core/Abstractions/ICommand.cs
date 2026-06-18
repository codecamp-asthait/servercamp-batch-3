using MediatR;

namespace Dukaan.Notification.Application.Core.Abstractions;

public interface ICommand<TResponse> : IRequest<TResponse> { }
public interface ICommand : IRequest { }
