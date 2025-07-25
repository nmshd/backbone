using System.ComponentModel.DataAnnotations;
using Backbone.BuildingBlocks.Infrastructure.Persistence.BlobStorage;
using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;

namespace Backbone.Modules.Files.Infrastructure;

public class InfrastructureConfiguration
{
    [Required]
    public required DatabaseConfiguration SqlDatabase { get; init; }

    [Required]
    public required BlobStorageOptions BlobStorage { get; init; }
}
