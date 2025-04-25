using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using FluentValidation;
using MediatR;
using ValidationException = Backbone.BuildingBlocks.Application.Abstractions.Exceptions.ValidationException;

namespace Backbone.BuildingBlocks.Application.MediatR;

public class RequestValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public RequestValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var context = new ValidationContext<TRequest>(request);

        var failures = _validators
            .Select(v => v.Validate(context))
            .SelectMany(result => result.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count != 0)
            throw new ValidationException(new ApplicationError(failures.First().ErrorCode,
                failures.First().ErrorMessage));

        return next(cancellationToken);
    }
}
