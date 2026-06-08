using FluentValidation;
using MediatR;

namespace learning_validation_mediatr.Behaviors;

/// <summary>
/// MediatR pipeline behavior that runs all registered FluentValidation validators
/// for a request before the handler is invoked.
/// Throws <see cref="ValidationException"/> if any rules fail.
/// </summary>
public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    /// <inheritdoc/>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var failures = validators
            .Select(v => v.Validate(request))
            .SelectMany(r => r.Errors)
            .Where(e => e is not null)
            .ToList();

        if (failures.Count != 0)
            throw new ValidationException(failures);

        return await next(cancellationToken);
    }
}
