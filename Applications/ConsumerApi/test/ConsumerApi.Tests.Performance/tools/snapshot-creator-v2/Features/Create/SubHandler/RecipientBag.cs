using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.SubHandler;

public record RecipientBag(RelationshipIdentityBag[] RelationshipIds, DomainIdentity[] RecipientIdentities);
