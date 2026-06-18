using MediatR;

namespace Dukaan.Application.Core.Abstractions;

public interface ICommandHandler<TCommand, TResponse> 
    : IRequestHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse> { }

public interface ICommandHandler<TCommand> 
    : IRequestHandler<TCommand>
    where TCommand : ICommand { }
