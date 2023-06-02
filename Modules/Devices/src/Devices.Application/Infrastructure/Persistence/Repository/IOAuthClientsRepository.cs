using Backbone.Modules.Devices.Application.Clients.DTOs;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Pagination;

namespace Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;

public interface IOAuthClientsRepository
{
    Task<DbPaginationResult<ClientDTO>> FindAll(PaginationFilter paginationFilter, CancellationToken cancellationToken);
}
