using System.ComponentModel.DataAnnotations;

namespace Backbone.BuildingBlocks.Application;

public class PaginationConfiguration
{
    [Required]
    [Range(1, 1000)]
    public required int MaxPageSize { get; init; }

    [Required]
    [Range(1, 1000)]
    public required int DefaultPageSize { get; init; }
}
