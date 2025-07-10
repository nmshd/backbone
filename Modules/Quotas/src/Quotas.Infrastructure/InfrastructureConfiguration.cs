using System.ComponentModel.DataAnnotations;
using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;

namespace Backbone.Modules.Quotas.Infrastructure;

public class InfrastructureConfiguration
{
    [Required]
    public required DatabaseConfiguration SqlDatabase { get; init; }
}
