using System.Reflection;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.MediatR;
using Backbone.Modules.Synchronization.Application.Datawallets.Commands.PushDatawalletModifications;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ValidationException = Backbone.BuildingBlocks.Application.Abstractions.Exceptions.ValidationException;

namespace Backbone.Modules.Synchronization.Application.Extensions;

public static class IServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public void AddApplication()
        {
            services.AddMediatR(c => c
                .RegisterServicesFromAssemblyContaining<PushDatawalletModificationsCommand>()
                .AddOpenBehavior(typeof(LoggingBehavior<,>))
                .AddOpenBehavior(typeof(RequestValidationBehavior<,>))
                .AddOpenBehavior(typeof(QuotaEnforcerBehavior<,>))
            );

            services.AddValidatorsFromAssembly(typeof(PushDatawalletModificationsCommand).Assembly);
            services.AddEventHandlers();
        }

        private void AddEventHandlers()
        {
            foreach (var eventHandler in GetAllDomainEventHandlers())
            {
                services.AddTransient(eventHandler);
            }
        }
    }

    private static IEnumerable<Type> GetAllDomainEventHandlers()
    {
        var domainEventHandlerTypes =
            from t in Assembly.GetExecutingAssembly().GetTypes()
            from i in t.GetInterfaces()
            where t.IsClass && t is { IsAbstract: false, IsGenericType: true } && i.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>)
            select t;

        return domainEventHandlerTypes;
    }
}

public class RequestValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public RequestValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var context = new ValidationContext<TRequest>(request);

        var failures = _validators
            .Select(v => v.Validate(context))
            .SelectMany(result => result.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count != 0)
            throw new ValidationException(new ApplicationError(failures.First().ErrorCode, failures.First().ErrorMessage));

        return next(cancellationToken);
    }
}
