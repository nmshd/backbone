using System.ComponentModel.DataAnnotations;
using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;

namespace Backbone.Modules.Challenges.Infrastructure;

public class InfrastructureConfiguration
{
    [Required]
    public required DatabaseConfiguration SqlDatabase { get; init; }
}
