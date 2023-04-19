using Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming;
using Backbone.Modules.Quotas.Domain.Aggregates.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Backbone.Modules.Quotas.Application.Tests.Tests.Identities
{
    public class HandlerTests
    {
        private readonly ILogger<IdentityCreatedIntegrationEventHandler> _logger;

        public HandlerTests() { }

        [Fact]
        public async void Returns_add_when_identities_created()
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
