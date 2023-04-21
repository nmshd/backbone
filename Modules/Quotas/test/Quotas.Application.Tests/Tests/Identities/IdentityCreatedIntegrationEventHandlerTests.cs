using Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Backbone.Modules.Quotas.Application.Tests.Tests.Identities
{
    public class IdentityCreatedIntegrationEventHandlerTests
    {
        private readonly ILogger<IdentityCreatedIntegrationEventHandler> _logger;

        public IdentityCreatedIntegrationEventHandlerTests() 
        {
            _logger = A.Fake< ILogger<IdentityCreatedIntegrationEventHandler>>();
        }

        [Fact]
        public async void Successfully_creates_identity_after_consuming_integration_event()
        {
            // Arrange
            var address = "id17RDEphijMPFGLbhqLWWgJfat";
            var tierId = "1";
            var mockIdentityRepository = new AddMockIdentityRepository();
            var handler = CreateHandler(mockIdentityRepository);

            // Act
            await handler.Handle(new IdentityCreatedIntegrationEvent() { Address = address, TierId = tierId });

            // Assert
            mockIdentityRepository.WasCalled.Should().BeTrue();
            mockIdentityRepository.WasCalledWith.Address.Should().Be("id17RDEphijMPFGLbhqLWWgJfat");
            mockIdentityRepository.WasCalledWith.TierId.Should().Be("1");
        }

        private IdentityCreatedIntegrationEventHandler CreateHandler(AddMockIdentityRepository identities)
        {
            return new IdentityCreatedIntegrationEventHandler(identities, _logger);
        }
    }
}
