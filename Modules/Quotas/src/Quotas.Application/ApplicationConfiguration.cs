using System.ComponentModel.DataAnnotations;

namespace Backbone.Modules.Quotas.Application;

public class ApplicationConfiguration
{
    [Required]
    public PaginationOptions Pagination { get; set; } = new();
}

public class PaginationOptions
{
    [Required]
    [Range(1, 1000)]
    public int MaxPageSize { get; set; }

    [Required]
    [Range(1, 1000)]
    public int DefaultPageSize { get; set; }
}
