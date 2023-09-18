namespace AdminUi.Tests.Integration.Models;
public class TierDetailsDTO
{
    public string Id { get; set; }
    public string Name { get; set; }
    public IEnumerable<TierQuotaDTO> Quotas { get; set; }
}
