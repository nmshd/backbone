using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.Entities;
using MediatR;

namespace Backbone.Modules.Synchronization.Application.Datawallets.Commands.CreateDatawallet;
public class Handler : IRequestHandler<CreateDatawalletCommand, CreateDatawalletResponse>
{
    private readonly IdentityAddress _activeIdentity;
    private readonly ISynchronizationDbContext _dbContext;

    public Handler(ISynchronizationDbContext dbContext, IUserContext userContext)
    {
        _dbContext = dbContext;
        _activeIdentity = userContext.GetAddress();
    }

    public async Task<CreateDatawalletResponse> Handle(CreateDatawalletCommand request, CancellationToken cancellationToken)
    {
        await EnsureNoDatawalletForActiveIdentityExists(cancellationToken);

        var datawallet = new Datawallet(new Datawallet.DatawalletVersion(request.DatawalletVersion), _activeIdentity);
        _dbContext.Set<Datawallet>().Add(datawallet);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new CreateDatawalletResponse
        {
            DatawalletId = datawallet.Id,
            Owner = datawallet.Owner,
            Version = datawallet.Version
        };
    }

    private async Task EnsureNoDatawalletForActiveIdentityExists(CancellationToken cancellationToken)
    {
        var datawallet = await _dbContext.GetDatawallet(_activeIdentity, cancellationToken);

        if (datawallet != null)
            throw new OperationFailedException(ApplicationErrors.Datawallet.InsufficientSupportedDatawalletVersion());
    }
}
