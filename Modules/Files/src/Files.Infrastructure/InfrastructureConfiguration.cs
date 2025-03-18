using System.ComponentModel.DataAnnotations;
using Backbone.BuildingBlocks.Infrastructure.Persistence.BlobStorage;
using Backbone.BuildingBlocks.Infrastructure.Persistence.Database;

namespace Backbone.Modules.Files.Infrastructure;

public class InfrastructureConfiguration
{
    [Required]
    public DatabaseConfiguration SqlDatabase { get; set; } = new();

    [Required]
    public BlobStorageOptions BlobStorage { get; set; } = new();
}
