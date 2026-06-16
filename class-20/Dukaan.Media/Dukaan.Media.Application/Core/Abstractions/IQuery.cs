using MediatR;

namespace Dukaan.Media.Application.Core.Abstractions;

public interface IQuery<TResponse> : IRequest<TResponse> { }
