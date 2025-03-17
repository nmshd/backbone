using System.ComponentModel.DataAnnotations;
using Backbone.BuildingBlocks.Application;

namespace Backbone.Modules.Synchronization.Application;

public class ApplicationConfiguration
{
    [Required]
    public PaginationConfiguration Pagination { get; set; } = new();
}
