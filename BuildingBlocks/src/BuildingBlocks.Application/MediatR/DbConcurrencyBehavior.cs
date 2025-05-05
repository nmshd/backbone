using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Backbone.BuildingBlocks.Application.MediatR;

public class DbConcurrencyBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly Random _random = new();

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var retryCount = 0;
        while (true)
        {
            try
            {
                return await next(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (retryCount == 10) throw;

                await Task.Delay(TimeSpan.FromMilliseconds(_random.Next(2, 10)), cancellationToken);
                retryCount++;
            }
        }
    }
}
