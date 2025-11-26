using Backbone.BuildingBlocks.Domain;
using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Tokens.Domain.Entities;

public class TokenAllocation : Entity
{
    [UsedImplicitly(Reason = "This constructor is for EF Core only")]
    protected TokenAllocation()
    {
        TokenId = null!;
        AllocatedBy = null!;
        AllocatedByDevice = null!;
    }

    public TokenAllocation(Token token, IdentityAddress allocatedBy, DeviceId allocatedByDevice)
    {
        TokenId = token.Id;
        AllocatedBy = allocatedBy;
        AllocatedByDevice = allocatedByDevice;
        AllocatedAt = DateTime.UtcNow;
    }

    public TokenId TokenId { get; set; }
    public IdentityAddress AllocatedBy { get; set; }
    public DeviceId AllocatedByDevice { get; set; }
    public DateTime AllocatedAt { get; set; }
}
