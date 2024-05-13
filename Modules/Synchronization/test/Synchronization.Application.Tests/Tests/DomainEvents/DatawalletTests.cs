using Backbone.Modules.Synchronization.Domain.DomainEvents.Outgoing;
using Backbone.UnitTestTools.FluentAssertions.Extensions;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Synchronization.Application.Tests.Tests.DomainEvents;

public class DatawalletTests
{
    [Fact]
    public void Raises_a_domain_event_on_adding_a_modification()
    {
        var identityAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var datawalletVersion = new Domain.Entities.Datawallet.DatawalletVersion(1);
        var datawallet = new Domain.Entities.Datawallet(datawalletVersion, identityAddress);
        var parameters = new DatawalletExtensions.AddModificationParameters();

        datawallet.AddModification(parameters);
        var domainEvent = datawallet.Should().HaveASingleDomainEvent<DatawalletModifiedDomainEvent>();
        domainEvent.Identity.Should().Be(identityAddress);
        domainEvent.ModifiedByDevice.Should().Be(parameters.CreatedByDevice);
    }
}
