using MediatR;

namespace Backbone.Modules.Quotas.Application.Tiers.Queries.GetTierById;
public class GetTierByIdQuery : IRequest<GetTierByIdResponse>
{
    public GetTierByIdQuery(string id)
    {
        Id = id;
    }
    public string Id { get; set; }
}
