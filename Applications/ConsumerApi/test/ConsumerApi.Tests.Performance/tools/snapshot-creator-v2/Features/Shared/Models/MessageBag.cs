namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;

public record MessageBag(string MessageId, string CreatedByDevice, DomainIdentity RecipientIdentity);
