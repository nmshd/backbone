using System.ComponentModel.DataAnnotations;
using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;

namespace Backbone.Modules.Relationships.Infrastructure;

public class InfrastructureConfiguration
{
    [Required]
    public DatabaseConfiguration SqlDatabase { get; set; } = new();
}
