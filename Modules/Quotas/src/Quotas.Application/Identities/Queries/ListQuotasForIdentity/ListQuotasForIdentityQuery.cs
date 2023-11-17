using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using MediatR;

namespace Backbone.Modules.Quotas.Application.Identities.Queries.GetQuotasForIdentity;

public class ListQuotasForIdentityQuery : IRequest<ListQuotasForIdentityResponse>
{
    public ListQuotasForIdentityQuery(IdentityAddress address)
    {
        Address = address;
    }

    public IdentityAddress Address { get; }
}
