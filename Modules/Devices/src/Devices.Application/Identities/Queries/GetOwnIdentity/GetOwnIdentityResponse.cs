using Backbone.Modules.Devices.Application.DTOs;
using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Application.Identities.Queries.GetOwnIdentity;
public class GetOwnIdentityResponse : OwnIdentityDTO
{
    public GetOwnIdentityResponse(Identity identity) : base(identity) { }
}
