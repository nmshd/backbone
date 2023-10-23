using Backbone.Devices.Application.DTOs;
using Backbone.Devices.Domain.Entities;

namespace Backbone.Devices.Application.Identities.Queries.GetIdentity;
public class GetIdentityResponse : IdentitySummaryDTO
{
    public GetIdentityResponse(Identity identity) : base(identity.Address, identity.ClientId, identity.PublicKey, identity.IdentityVersion, identity.CreatedAt, identity.Devices, identity.TierId) { }
}
