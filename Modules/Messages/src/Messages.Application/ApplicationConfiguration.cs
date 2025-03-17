using System.ComponentModel.DataAnnotations;
using Backbone.BuildingBlocks.Application;

namespace Backbone.Modules.Messages.Application;

public class ApplicationConfiguration
{
    [Range(1, 100)]
    public int MaxNumberOfUnreceivedMessagesFromOneSender { get; set; }

    [Required]
    public PaginationConfiguration Pagination { get; set; } = new();

    [Required]
    [MinLength(3)]
    [MaxLength(45)]
    public string DidDomainName { get; set; } = null!;

    [Required]
    public int MaxNumberOfMessageRecipients { get; set; }
}
