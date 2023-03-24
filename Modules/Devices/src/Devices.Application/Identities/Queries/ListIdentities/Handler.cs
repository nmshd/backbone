using Backbone.Modules.Devices.Application.DTOs;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence;
using Backbone.Modules.Devices.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Devices.Application.Identities.Queries.ListIdentities;
public class Handler : IRequestHandler<ListIdentitiesQuery, ListIdentitiesResponse>
{
    private readonly IDevicesDbContext _dbContext;
#nullable enable
    private readonly ILogger<Handler>? _logger;
#nullable disable
    public Handler(IDevicesDbContext dbContext,
        ILogger<Handler> logger
        )
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    public Task<ListIdentitiesResponse> Handle(ListIdentitiesQuery request, CancellationToken cancellationToken)
    {
        _logger?.LogTrace("Getting all Identities");
        var identities = _dbContext.Set<Identity>().ToList();
        var identitiesDTOList = identities.Select(el =>
        {
            return new IdentityDTO(el.Address, el.ClientId, el.PublicKey, el.IdentityVersion, el.CreatedAt);
        }).ToList();

        return Task.FromResult(new ListIdentitiesResponse(identitiesDTOList));
     }
}
