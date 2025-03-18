using System.ComponentModel.DataAnnotations;
using Backbone.BuildingBlocks.Application;

namespace Backbone.Modules.Relationships.Application;

public class ApplicationConfiguration
{
    [Required]
    public PaginationConfiguration Pagination { get; set; } = new();

    [Required]
    [MinLength(3)]
    [MaxLength(45)]
    public string DidDomainName { get; set; } = null!;
}
