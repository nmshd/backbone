using System.ComponentModel.DataAnnotations;
using Backbone.BuildingBlocks.Application;

namespace Backbone.Modules.Messages.Application;

public class ApplicationConfiguration
{
    [Range(1, 100)]
    public int MaxNumberOfUnreceivedMessagesFromOneSender { get; set; }

    [Required]
    public required PaginationConfiguration Pagination { get; init; }

    [Required]
    [MinLength(3)]
    [MaxLength(45)]
    public required string DidDomainName { get; init; }

    [Required]
    public required int MaxNumberOfMessageRecipients { get; init; }
}
