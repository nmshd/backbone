using System.ComponentModel.DataAnnotations;
using Backbone.Modules.Tokens.Infrastructure.Persistence.Database;

namespace Backbone.Modules.Tokens.Module;

public class InfrastructureConfiguration
{
    [Required]
    public DbOptions SqlDatabase { get; set; } = new();
}
