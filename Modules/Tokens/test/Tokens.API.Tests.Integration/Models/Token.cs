using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Tokens.API.Tests.Integration.Models;
public class Token
{
    [Required]
    public string Id { get; set; }
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public DateTime? ExpiresAt { get; set; }
    [Required]
    public DateTime CreatedAt { get; set; }
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string? CreatedBy { get; set; }
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string? CreatedByDevice { get; set; }
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string? Content { get; set; }
}
