using Backbone.Modules.Synchronization.Domain.DomainEvents.Outgoing;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Backbone.UnitTestTools.Shouldly.Extensions;

namespace Backbone.Modules.Synchronization.Domain.Tests;

public class ExternalEventTests : AbstractTestsBase
{
    [Fact]
    public void Raises_a_domain_event_on_initialization()
    {
        var identityAddress = CreateRandomIdentityAddress();
        var externalEvent = new IdentityDeletionProcessStatusChangedExternalEvent(identityAddress, new IdentityDeletionProcessStatusChangedExternalEvent.EventPayload { DeletionProcessId = "" });

        var createdDomainEvent = externalEvent.ShouldHaveASingleDomainEvent<ExternalEventCreatedDomainEvent>();
        createdDomainEvent.EventId.ShouldBe(externalEvent.Id.Value);
        createdDomainEvent.Owner.ShouldBe(externalEvent.Owner.Value);
    }
}
