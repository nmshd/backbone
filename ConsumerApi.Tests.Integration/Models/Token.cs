using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace ConsumerApi.Tests.Integration.Models;

public class Token
{
    [Required]
    public string Id { get; set; }
    public DateTime? ExpiresAt { get; set; }
    [Required]
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? CreatedByDevice { get; set; }
    public string? Content { get; set; }

    public bool ShouldSerializeExpiresAt()
    {
        return ExpiresAt != null;
    }

    public bool ShouldSerializeCreatedBy()
    {
        return CreatedBy != null;
    }

    public bool ShouldSerializeCreatedByDevice()
    {
        return CreatedByDevice != null;
    }

    public bool ShouldSerializeContent()
    {
        return Content != null;
    }
}
