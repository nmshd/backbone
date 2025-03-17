using System.ComponentModel.DataAnnotations;
using Backbone.Modules.Synchronization.Infrastructure.Persistence.Database;

namespace Backbone.Modules.Synchronization.Infrastructure;

public class InfrastructureConfiguration
{
    [Required]
    public DbOptions SqlDatabase { get; set; } = new();
}
