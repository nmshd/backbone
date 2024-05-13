using Backbone.Modules.Synchronization.Domain.DomainEvents.Outgoing;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Backbone.UnitTestTools.FluentAssertions.Extensions;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Synchronization.Application.Tests.Tests.DomainEvents;

public class ExternalEventTests
{
    [Fact]
    public void Raises_a_domain_event_on_initialization()
    {
        var identityAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var externalEvent = new ExternalEvent(ExternalEventType.IdentityDeletionProcessStatusChanged, identityAddress, 1, new { });

        var createdDomainEvent = externalEvent.Should().HaveASingleDomainEvent<ExternalEventCreatedDomainEvent>();
        createdDomainEvent.EventId.Should().Be(externalEvent.Id.Value);
        createdDomainEvent.Owner.Should().Be(externalEvent.Owner.Value);
    }
}
