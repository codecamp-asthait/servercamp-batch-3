using MediatR;

namespace Dukaan.Application.Core.Abstractions;

public interface IQueryHandler<TQuery, TResponse> 
    : IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse> { }
