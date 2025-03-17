using System.ComponentModel.DataAnnotations;
using Backbone.Modules.Relationships.Infrastructure.Persistence.Database;

namespace Backbone.Modules.Relationships.Infrastructure;

public class InfrastructureConfiguration
{
    [Required]
    public IServiceCollectionExtensions.DbOptions SqlDatabase { get; set; } = new();
}

public class PublicRelationshipTemplateReferenceDefinition
{
    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    public string TruncatedReference { get; set; } = string.Empty;
}
