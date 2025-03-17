using System.ComponentModel.DataAnnotations;

namespace Backbone.Modules.Tokens.Application;

public class ApplicationConfiguration
{
    [Required]
    public PaginationOptions Pagination { get; set; } = new();

    [Required]
    [MinLength(3)]
    [MaxLength(45)]
    public string DidDomainName { get; set; } = null!;
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
