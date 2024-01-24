using Backbone.Modules.Devices.Application.DTOs;
using Backbone.Modules.Devices.Domain.Entities;

namespace Backbone.Modules.Devices.Application.Identities.Queries.GetIdentity;
public class GetIdentityResponse : IdentitySummaryDTO
{
    public GetIdentityResponse(Identity identity) : base(identity) { }
}
