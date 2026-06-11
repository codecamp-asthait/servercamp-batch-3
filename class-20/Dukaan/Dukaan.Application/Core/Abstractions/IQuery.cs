using MediatR;

namespace Dukaan.Application.Core.Abstractions;

public interface IQuery<TResponse> : IRequest<TResponse> { }
