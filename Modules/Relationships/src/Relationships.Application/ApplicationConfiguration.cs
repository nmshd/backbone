using System.ComponentModel.DataAnnotations;
using Backbone.BuildingBlocks.Application;

namespace Backbone.Modules.Relationships.Application;

public class ApplicationConfiguration
{
    [Required]
    public required PaginationConfiguration Pagination { get; init; }

    [Required]
    [MinLength(3)]
    [MaxLength(45)]
    public required string DidDomainName { get; init; }
}
