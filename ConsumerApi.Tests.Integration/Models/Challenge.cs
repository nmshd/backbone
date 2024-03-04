using System.ComponentModel.DataAnnotations;

namespace Backbone.ConsumerApi.Tests.Integration.Models;

public class Challenge
{
    [Required]
    public required string Id { get; set; }
    [Required]
    public required DateTime ExpiresAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? CreatedByDevice { get; set; }
}
