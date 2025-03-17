using System.ComponentModel.DataAnnotations;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Database;

namespace Backbone.Modules.Quotas.Infrastructure;

public class InfrastructureConfiguration
{
    [Required]
    public IServiceCollectionExtensions.DbOptions SqlDatabase { get; set; } = new();
}
