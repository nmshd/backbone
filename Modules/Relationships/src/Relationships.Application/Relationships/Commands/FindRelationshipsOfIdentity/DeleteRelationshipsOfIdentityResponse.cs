using Backbone.BuildingBlocks.Application.CQRS.BaseClasses;
using Backbone.Modules.Relationships.Domain.Entities;

namespace Backbone.Modules.Relationships.Application.Relationships.Commands.FindRelationshipsOfIdentity;

public class FindRelationshipsOfIdentityResponse(IEnumerable<Relationship> items) : CollectionResponseBase<Relationship>(items);
