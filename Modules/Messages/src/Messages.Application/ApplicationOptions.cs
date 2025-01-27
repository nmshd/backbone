using System.ComponentModel.DataAnnotations;

namespace Backbone.Modules.Messages.Application;

public class ApplicationOptions
{
    [Range(1, 100)]
    public int MaxNumberOfUnreceivedMessagesFromOneSender { get; set; }

    [Required]
    public PaginationOptions Pagination { get; set; } = new();

    [Required]
    [MinLength(3)]
    [MaxLength(45)]
    public string DidDomainName { get; set; } = null!;

    [Required]
    public int MaxNumberOfMessageRecipients { get; set; }
}

public class PaginationOptions
{
    [Range(1, 1000)]
    public int MaxPageSize { get; set; }

    [Range(1, 1000)]
    public int DefaultPageSize { get; set; }
}
