using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Devices.Module.Features.Shared;

namespace Backbone.Modules.Devices.Module.Features.Identities.GetIdentity;

public class Response : IdentitySummaryDTO
{
    public Response(Identity identity) : base(identity) { }
}
