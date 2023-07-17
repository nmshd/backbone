using System.ComponentModel.DataAnnotations;

namespace Backbone.Modules.Devices.Application;

public class ApplicationOptions
{
    [Required]
    [MinLength(3)]
    [MaxLength(3)]
    public string AddressPrefix { get; set; }

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
