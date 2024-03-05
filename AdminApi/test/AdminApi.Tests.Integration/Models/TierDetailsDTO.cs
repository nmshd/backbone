namespace Backbone.AdminApi.Tests.Integration.Models;
public class TierDetailsDTO
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required IEnumerable<TierQuotaDTO> Quotas { get; set; }
}
