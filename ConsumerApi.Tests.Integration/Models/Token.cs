using System.ComponentModel.DataAnnotations;

namespace Backbone.ConsumerApi.Tests.Integration.Models;

public class Token
{
    [Required]
    public required string Id { get; set; }
    public DateTime? ExpiresAt { get; set; }
    [Required]
    public required DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? CreatedByDevice { get; set; }
    public string? Content { get; set; }
}
