using Backbone.BuildingBlocks.SDK.Endpoints.Common.Types;

namespace Backbone.AdminApi.Sdk.Endpoints.Tokens.Response;

public class ListTokensResponse : EnumerableResponseBase<Token>
{
    public PaginationData? Pagination { get; set; }
}

public class Token
{
    public required string Id { get; set; }

    public required string CreatedBy { get; set; }
    public required string CreatedByDevice { get; set; }

    public required string? ForIdentity { get; set; }

    public required DateTime CreatedAt { get; set; }
    public required DateTime ExpiresAt { get; set; }

    public required byte[] Content { get; set; }

    public required bool IsPasswordProtected { get; set; }
}
