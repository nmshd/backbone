using System.ComponentModel.DataAnnotations;
using Backbone.Modules.Messages.Infrastructure.Persistence.Database;

namespace Backbone.Modules.Messages.Module;

public class InfrastructureConfiguration
{
    [Required]
    public DbOptions SqlDatabase { get; set; } = new();
}
