using Backbone.Quotas.Application.DTOs;
using MediatR;

namespace Backbone.Quotas.Application.Tiers.Queries.GetTierById;
public class GetTierByIdQuery : IRequest<TierDetailsDTO>
{
    public GetTierByIdQuery(string id)
    {
        Id = id;
    }
    public string Id { get; set; }
}
