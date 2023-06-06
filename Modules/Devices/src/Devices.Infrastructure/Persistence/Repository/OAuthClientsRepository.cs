using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Core;
using OpenIddict.EntityFrameworkCore.Models;

namespace Backbone.Modules.Devices.Infrastructure.Persistence.Repository;
public class OAuthClientsRepository : IOAuthClientsRepository
{
    private readonly OpenIddictApplicationManager<OpenIddictEntityFrameworkCoreApplication> _applicationManager;
    
    public OAuthClientsRepository(OpenIddictApplicationManager<OpenIddictEntityFrameworkCoreApplication> applicationManager)
    {
        _applicationManager = applicationManager;
    }

    public async Task<IEnumerable<OAuthClient>> FindAll(CancellationToken cancellationToken)
    {
        var clients = await _applicationManager.ListAsync(applications => applications.Select(c => new OAuthClient(c.ClientId, c.DisplayName)), cancellationToken).ToListAsync(cancellationToken);

        return clients;
    }
}
