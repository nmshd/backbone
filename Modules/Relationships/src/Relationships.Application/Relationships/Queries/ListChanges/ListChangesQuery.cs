using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Common;
using Backbone.Modules.Relationships.Domain.Entities;
using Backbone.Modules.Relationships.Domain.Ids;
using MediatR;

namespace Backbone.Modules.Relationships.Application.Relationships.Queries.ListChanges;

public class ListChangesQuery : IRequest<ListChangesResponse>
{
    public ListChangesQuery(PaginationFilter paginationFilter, IEnumerable<RelationshipChangeId> ids, OptionalDateRange? createdAt, OptionalDateRange? completedAt, OptionalDateRange? modifiedAt, RelationshipChangeStatus? status, RelationshipChangeType? type, IdentityAddress? createdBy, IdentityAddress? completedBy, bool onlyPeerChanges)
    {
        PaginationFilter = paginationFilter;
        Ids = ids;
        CreatedAt = createdAt;
        CompletedAt = completedAt;
        ModifiedAt = modifiedAt;
        Status = status;
        Type = type;
        CreatedBy = createdBy;
        CompletedBy = completedBy;
        OnlyPeerChanges = onlyPeerChanges;
    }

    public PaginationFilter PaginationFilter { get; set; }
    public IEnumerable<RelationshipChangeId> Ids { get; set; }
    public OptionalDateRange? CreatedAt { get; set; }
    public OptionalDateRange? CompletedAt { get; set; }
    public OptionalDateRange? ModifiedAt { get; }
    public RelationshipChangeStatus? Status { get; }
    public IdentityAddress? CreatedBy { get; set; }
    public IdentityAddress? CompletedBy { get; set; }
    public bool OnlyPeerChanges { get; set; }
    public RelationshipChangeType? Type { get; set; }
}
