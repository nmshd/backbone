using Backbone.Modules.Devices.Application.DTOs;
using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Application.Identities.Queries.GetRegisteredIdentityDetails;
public class GetRegisteredIdentityDetailsResponse : RegisteredIdentityDetailsDTO
{
    public GetRegisteredIdentityDetailsResponse(Identity identity) : base(identity) { }
}
