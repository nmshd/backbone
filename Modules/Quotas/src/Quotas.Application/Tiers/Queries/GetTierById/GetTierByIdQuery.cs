using Backbone.Modules.Quotas.Application.DTOs;
using MediatR;

namespace Backbone.Modules.Quotas.Application.Tiers.Queries.GetTierById;
public class GetTierByIdQuery : IRequest<TierDetailsDTO>
{
    public GetTierByIdQuery(string id)
    {
        Id = id;
    }
    public string Id { get; set; }
}
