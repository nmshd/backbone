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
            var mockIdentitiesRepository = new AddMockIdentitiesRepository();
            var handler = CreateHandler(mockIdentitiesRepository);

            // Act
            await handler.Handle(new IdentityCreatedIntegrationEvent(address, tier));

            // Assert
            mockIdentitiesRepository.WasCalled.Should().BeTrue();
            mockIdentitiesRepository.WasCalledWith.Address.Should().Be(address);
            mockIdentitiesRepository.WasCalledWith.TierId.Should().Be(tier);
        }

        private IdentityCreatedIntegrationEventHandler CreateHandler(AddMockIdentitiesRepository identities)
        {
            var logger = A.Fake<ILogger<IdentityCreatedIntegrationEventHandler>>();
            return new IdentityCreatedIntegrationEventHandler(identities, logger);
        }
    }
}
