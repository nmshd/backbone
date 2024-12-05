using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.Models;

public record RecipientBag(RelationshipIdentityBag[] RelationshipIdentityBags, DomainIdentity[] RecipientDomainIdentities);
