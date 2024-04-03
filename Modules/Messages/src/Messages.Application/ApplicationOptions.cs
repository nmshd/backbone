using System.ComponentModel.DataAnnotations;

namespace Backbone.Modules.Messages.Application;

public class ApplicationOptions
{
    [Required]
    [MinLength(3)]
    [MaxLength(3)]
    public string AddressPrefix { get; set; } = null!;

    [Range(1, 100)]
    public int MaxNumberOfUnreceivedMessagesFromOneSender { get; set; }

    [Required]
    public PaginationOptions Pagination { get; set; } = new();
}

public class PaginationOptions
{
    [Range(1, 1000)]
    public int MaxPageSize { get; set; }

    [Range(1, 1000)]
    public int DefaultPageSize { get; set; }
}
