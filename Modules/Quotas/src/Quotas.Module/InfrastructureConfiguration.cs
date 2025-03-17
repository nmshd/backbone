using System.ComponentModel.DataAnnotations;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Database;

namespace Backbone.Modules.Quotas.Module;

public class InfrastructureConfiguration
{
    [Required]
    public IServiceCollectionExtensions.DbOptions SqlDatabase { get; set; } = new();
}
