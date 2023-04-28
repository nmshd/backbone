using Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.IdentityCreated;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Backbone.Modules.Quotas.Application.Tests.Tests.Identities
{
    public class IdentityCreatedIntegrationEventHandlerTests
    {
        [Fact]
        public async void Successfully_creates_identity_after_consuming_integration_event()
        {
            // Arrange
            var address = "id12Pbi7CgBHaFxge6uy1h6qUkedjQr8XHfm";
            var tier = "TIRFxoL0U24aUqZDSAWc";
            var mockIdentityRepository = new AddMockIdentitiesRepository();
            var handler = CreateHandler(mockIdentityRepository);

            // Act
            await handler.Handle(new IdentityCreatedIntegrationEvent(address, tier));

            // Assert
            mockIdentityRepository.WasCalled.Should().BeTrue();
            mockIdentityRepository.WasCalledWith.Address.Should().Be("id12Pbi7CgBHaFxge6uy1h6qUkedjQr8XHfm");
            mockIdentityRepository.WasCalledWith.TierId.Should().Be("TIRFxoL0U24aUqZDSAWc");
        }

        private IdentityCreatedIntegrationEventHandler CreateHandler(AddMockIdentitiesRepository identities)
        {
            var logger = A.Fake<ILogger<IdentityCreatedIntegrationEventHandler>>();
            return new IdentityCreatedIntegrationEventHandler(identities, logger);
        }
    }
}
