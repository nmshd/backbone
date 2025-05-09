using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Backbone.BuildingBlocks.Application.MediatR;

public class DbConcurrencyBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private static readonly Random RANDOM = new();

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

                await Task.Delay(TimeSpan.FromSeconds(RANDOM.Next(5, 10)), cancellationToken);
                retryCount++;
            }
        }
    }
}
