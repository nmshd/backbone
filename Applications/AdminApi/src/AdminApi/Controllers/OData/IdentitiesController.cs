using Backbone.AdminApi.Infrastructure.DTOs;
using Backbone.AdminApi.Infrastructure.Persistence.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Backbone.AdminApi.Controllers.OData;

[Authorize("ApiKey")]
public class IdentitiesController : ODataController
{
    private readonly AdminApiDbContext _adminApiDbContext;

    public IdentitiesController(AdminApiDbContext adminApiDbContext)
    {
        _adminApiDbContext = adminApiDbContext;
    }

    [EnableQuery]
    public IQueryable<IdentityOverview> Get()
    {
        return _adminApiDbContext.Identities.Select(i => new IdentityOverview
        {
            Address = i.Address,
            IdentityVersion = i.IdentityVersion,
            CreatedAt = i.CreatedAt,
            Tier = _adminApiDbContext.Tiers.Select(t => new TierDTO { Id = t.Id, Name = t.Name }).First(t => t.Id == i.TierId),
            DatawalletVersion = _adminApiDbContext.Datawallets.Where(d => d.Owner == i.Address).Select(d => d.Version).First(),
            CreatedWithClient = i.ClientId,
            LastLoginAt = i.Devices.Max(d => d.User.LastLoginAt),
            NumberOfDevices = i.Devices.Count
        });
    }
}
