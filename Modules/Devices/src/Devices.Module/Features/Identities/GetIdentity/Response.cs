using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Devices.Module.Features.Shared;

namespace Backbone.Modules.Devices.Module.Features.Identities.GetIdentity;

public class GetIdentityResponse : IdentitySummaryDTO
{
    public GetIdentityResponse(Identity identity) : base(identity) { }
}
