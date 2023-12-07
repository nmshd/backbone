using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Synchronization.Application.Datawallets.Commands.DeleteDatawalletsByIdentity;
public class Handler(ISynchronizationDbContext dbContext) : IRequestHandler<DeleteDatawalletsByIdentityCommand>
{
    public async Task Handle(DeleteDatawalletsByIdentityCommand request, CancellationToken cancellationToken)
    {
        await dbContext.Set<Datawallet>().Where(d => d.Owner == request.IdentityAddress).ExecuteDeleteAsync(cancellationToken);
    }
}
