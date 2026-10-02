using MediatR;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.ListRelationshipsOfIdentity;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("ListRelationshipsOfIdentityQuery")]
public class Query : IRequest<Response>
{
    public required string IdentityAddress { get; init; }
}
