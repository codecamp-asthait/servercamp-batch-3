using MediatR;

namespace Dukaan.Notification.Application.Core.Abstractions;

public interface IQuery<TResponse> : IRequest<TResponse> { }
