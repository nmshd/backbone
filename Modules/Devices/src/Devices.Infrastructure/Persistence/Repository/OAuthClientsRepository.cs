using Backbone.Modules.Devices.Application.Clients.DTOs;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Pagination;
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

    public async Task<DbPaginationResult<ClientDTO>> FindAll(PaginationFilter paginationFilter, CancellationToken cancellationToken)
    {
        var clients = await _applicationManager.ListAsync(applications => applications.Select(c => new ClientDTO(c.ClientId, c.DisplayName)), cancellationToken).ToListAsync();
        var paginationResult = new DbPaginationResult<ClientDTO>(clients, clients.Count);

        return paginationResult;
    }
}
