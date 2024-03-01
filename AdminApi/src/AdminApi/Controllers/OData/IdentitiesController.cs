using Backbone.AdminApi.Infrastructure.DTOs;
using Backbone.AdminApi.Infrastructure.Persistence.Database;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Backbone.AdminApi.Controllers.OData;

public class IdentitiesController : ODataController
{
    private readonly AdminUiDbContext _adminUiDbContext;

    public IdentitiesController(AdminUiDbContext adminUiDbContext)
    {
        _adminUiDbContext = adminUiDbContext;
    }

    [EnableQuery]
    public IQueryable<IdentityOverview> Get()
    {
        return _adminUiDbContext.IdentityOverviews;
    }
}
