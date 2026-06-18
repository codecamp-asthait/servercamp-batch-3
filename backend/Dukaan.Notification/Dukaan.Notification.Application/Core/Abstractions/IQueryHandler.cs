using MediatR;

namespace Dukaan.Notification.Application.Core.Abstractions;

public interface IQueryHandler<TQuery, TResponse>
    : IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse> { }
