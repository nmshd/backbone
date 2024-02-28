using Backbone.ConsumerApi.Sdk.Endpoints.Common.Types;

namespace Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types;

public class ListRelationshipChangesParameters
{
    public PaginationFilter? Pagination { get; set; } = null;
    public List<string>? Ids { get; set; } = null;
    public OptionalDateRange? CreatedAt { get; set; } = null;
    public OptionalDateRange? CompletedAt { get; set; } = null;
    public OptionalDateRange? ModifiedAt { get; set; } = null;
    public bool? OnlyPeerChanges { get; set; } = null;
    public string? CreatedBy { get; set; } = null;
    public string? CompletedBy { get; set; } = null;
    public string? Status { get; set; } = null;
    public string? Type { get; set; } = null;
}

public class OptionalDateRange
{
    public DateTime? From { get; set; } = null;
    public DateTime? To { get; set; } = null;
}
