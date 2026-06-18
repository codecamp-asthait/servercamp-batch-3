using ErrorOr;
using FluentValidation;
using MediatR;

namespace Dukaan.Notification.Application.Core.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count == 0)
            return await next();

        var errors = failures
            .Select(f => Error.Validation(f.PropertyName, f.ErrorMessage))
            .ToList();

        return CreateErrorOrResult(errors);
    }

    private static TResponse CreateErrorOrResult(List<Error> errors)
    {
        var responseType = typeof(TResponse);
        var innerType = responseType.GetGenericArguments().First();
        var errorOrType = typeof(ErrorOr<>).MakeGenericType(innerType);

        var fromErrorsMethod = errorOrType.GetMethod("From", new[] { typeof(List<Error>) });
        var result = fromErrorsMethod!.Invoke(null, new object[] { errors });

        return (TResponse)result!;
    }
}
