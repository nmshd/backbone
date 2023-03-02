using System.ComponentModel.DataAnnotations;

namespace SpecFlowChallenges.Specs.Models;
public class ChallengeResult
{
    [Required]
    public string Id { get; set; }
    [Required]
    public DateTime ExpiresAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? CreatedByDevice { get; set; }
}
