using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Outgoing;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Synchronization.Application.Tests.Tests.DomainEvents;

public class ExternalEventCreatedDomainEventTests
{
    [Fact]
    public void Raises_a_domain_event_on_initialization()
    {
        var identityAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var externalEvent = new ExternalEvent(ExternalEventType.IdentityDeletionProcessStatusChanged, identityAddress, 1, new { });

        externalEvent.DomainEvents.Should().HaveCount(1);
        externalEvent.DomainEvents[0].Should().BeOfType<ExternalEventCreatedDomainEvent>();

        var createdDomainEvent = (ExternalEventCreatedDomainEvent)externalEvent.DomainEvents[0];
        createdDomainEvent.EventId.Should().Be(externalEvent.Id.Value);
        createdDomainEvent.Owner.Should().Be(externalEvent.Owner.Value);
    }
}
