using Backbone.Modules.Devices.Application.DTOs;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Queries.GetDeletionProcessAsSupport;
public class GetDeletionProcessAsSupportQuery: IRequest<IdentityDeletionProcessDTO>
{
    public GetDeletionProcessAsSupportQuery(string address, string id)
    {
        Address = address;
        Id = id;
    }

    public string Address { get; set; }
    public string Id { get; set; }
}
