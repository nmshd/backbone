using Backbone.Modules.Devices.Application.DTOs;

namespace Backbone.Modules.Devices.Application.Identities.Queries.ListIdentities;
public class ListIdentitiesResponse
{
    public ListIdentitiesResponse(List<IdentityDTO> identitiesDTOList)
    {
        Identities = identitiesDTOList;
    }

    public List<IdentityDTO> Identities { get; set; }
}
