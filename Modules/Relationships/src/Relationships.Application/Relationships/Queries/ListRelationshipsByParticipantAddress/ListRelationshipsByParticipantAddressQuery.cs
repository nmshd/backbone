using Enmeshed.BuildingBlocks.Application.Pagination;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Queries.ListRelationshipsByParticipantAddress;
public class ListRelationshipsByParticipantAddressQuery : IRequest<ListRelationshipsByParticipantAddressResponse>
{
    public ListRelationshipsByParticipantAddressQuery(PaginationFilter paginationFilter, IdentityAddress participantAddress)
    {
        PaginationFilter = paginationFilter;
        ParticipantAddress = participantAddress;
    }

    public PaginationFilter PaginationFilter { get; set; }
    public IdentityAddress ParticipantAddress { get; set; }
}
