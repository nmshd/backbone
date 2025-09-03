using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.BuildingBlocks.Application.MediatR;
using Backbone.Modules.Challenges.Application.Challenges.Commands.CreateChallenge;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ValidationException = Backbone.BuildingBlocks.Application.Abstractions.Exceptions.ValidationException;

namespace Backbone.Modules.Challenges.Application.Extensions;

public static class ApplicationServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(c => c
            .RegisterServicesFromAssemblyContaining<CreateChallengeCommand>()
            .AddOpenBehavior(typeof(LoggingBehavior<,>))
            .AddOpenBehavior(typeof(RequestValidationBehavior<,>))
            .AddOpenBehavior(typeof(QuotaEnforcerBehavior<,>))
        );
        services.AddValidatorsFromAssembly(typeof(CreateChallengeCommandValidator).Assembly);

        services.AddHousekeeper<Housekeeper>();
    }
}

public class RequestValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
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
