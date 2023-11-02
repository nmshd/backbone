using Backbone.AdminUi.Infrastructure.DTOs;
using Backbone.AdminUi.Infrastructure.Persistence.Database;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Backbone.AdminUi.Controllers.OData;

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
