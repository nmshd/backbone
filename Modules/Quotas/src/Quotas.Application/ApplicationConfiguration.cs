using System.ComponentModel.DataAnnotations;
using Backbone.BuildingBlocks.Application;

namespace Backbone.Modules.Quotas.Application;

public class ApplicationConfiguration
{
    [Required]
    public required PaginationConfiguration Pagination { get; init; }
}
