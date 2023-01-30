using Backbone.Modules.Relationships.Domain;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using MediatR;
using ApplicationException = Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace Backbone.Modules.Relationships.Application.MediatR;

public class DomainExceptionTransformerPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (DomainException ex)
        {
            throw new ApplicationException(new ApplicationError(ex.Error.Code, ex.Error.Message), ex);
        }
    }
}
