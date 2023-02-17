using System.ComponentModel.DataAnnotations;

namespace Challenges.API.Client.Models;
public class ChallengeResult
{
    [Required]
    public string Id { get; set; }
    [Required]
    public DateTime ExpiresAt { get; set; }
#nullable enable
    public string? CreatedBy { get; set; }
    public string? CreatedByDevice { get; set; }
}
