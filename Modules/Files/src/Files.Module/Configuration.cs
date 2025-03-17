using System.ComponentModel.DataAnnotations;
using Backbone.BuildingBlocks.Infrastructure.Persistence.BlobStorage;
using Backbone.Modules.Files.Infrastructure.Persistence.Database;

namespace Backbone.Modules.Files.Module;

public class InfrastructureConfiguration
{
    [Required]
    public DbOptions SqlDatabase { get; set; } = new();

    [Required]
    public BlobStorageOptions BlobStorage { get; set; } = new();
}
