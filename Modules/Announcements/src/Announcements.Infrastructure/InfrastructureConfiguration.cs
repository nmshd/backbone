using System.ComponentModel.DataAnnotations;
using Backbone.Modules.Announcements.Infrastructure.Persistence.Database;

namespace Backbone.Modules.Announcements.Infrastructure;

public class InfrastructureConfiguration
{
    [Required]
    public IServiceCollectionExtensions.DbOptions SqlDatabase { get; set; } = new();
}
