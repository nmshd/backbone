using Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming;
using Backbone.Modules.Quotas.Domain.Aggregates.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Backbone.Modules.Quotas.Application.Tests.Tests.Identities
{
    public class IdentityCreatedIntegrationEventHandlerTests
    {
        private readonly ILogger<IdentityCreatedIntegrationEventHandler> _logger;

        public IdentityCreatedIntegrationEventHandlerTests() { }

        [Fact]
        public async void Successfully_creates_identity_after_consuming_integration_event()
        {
            // Arrange
            var address = "id17RDEphijMPFGLbhqLWWgJfat";
            var tierId = "1";
            var identities = new Identity(address, tierId);

            var handler = CreateHandler(new AddIdentityRepository(identities));

            // Act
            var result = handler.Handle(new IdentityCreatedIntegrationEvent() { Address = address, TierId = tierId });

            // Assert
            result.Should().NotBeNull();

        }

        private IdentityCreatedIntegrationEventHandler CreateHandler(AddIdentityRepository identities)
        {
            return new IdentityCreatedIntegrationEventHandler(identities, _logger);
        }
    }
}
