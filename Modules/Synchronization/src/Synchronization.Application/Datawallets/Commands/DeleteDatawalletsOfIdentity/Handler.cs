using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Synchronization.Application.Datawallets.Commands.DeleteDatawalletsOfIdentity;
public class Handler(ISynchronizationDbContext dbContext) : IRequestHandler<DeleteDatawalletsOfIdentityCommand>
{
    public async Task Handle(DeleteDatawalletsOfIdentityCommand request, CancellationToken cancellationToken)
    {
        await dbContext.Set<Datawallet>().Where(d => d.Owner == request.IdentityAddress).ExecuteDeleteAsync(cancellationToken);
    }
}
