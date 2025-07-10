using System.ComponentModel.DataAnnotations;
using Backbone.BuildingBlocks.Application;

namespace Backbone.Modules.Synchronization.Application;

public class ApplicationConfiguration
{
    [Required]
    public required PaginationConfiguration Pagination { get; init; }
}
