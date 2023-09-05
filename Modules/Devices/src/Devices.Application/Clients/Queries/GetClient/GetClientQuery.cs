using MediatR;

namespace Backbone.Modules.Devices.Application.Clients.Queries.GetClient;
public class GetClientQuery : IRequest<GetClientResponse>
{
    public GetClientQuery(string id)
    {
        Id = id;
    }
    public string Id { get; set; }
}
