using ErrorOr;
using FluentValidation;
using MediatR;

namespace PantryCloud.IdentityService.Application.Validators;

public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
        {
            return await next(cancellationToken);
        }

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var errors = validationResults
            .SelectMany(r => r.Errors)
            .Where(e => e is not null)
            .Select(e => Error.Validation(e.PropertyName, e.ErrorMessage))
            .ToList();

        if (errors.Any())
        {
            return (dynamic)errors;
        }

        return await next(cancellationToken);
    }
}