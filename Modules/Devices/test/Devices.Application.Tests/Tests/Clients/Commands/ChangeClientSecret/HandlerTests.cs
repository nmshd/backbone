using Backbone.Modules.Devices.Application.Clients.Commands.ChangeClientSecret;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities;
using Backbone.Tooling;
using FakeItEasy;
using Xunit;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Clients.Commands.ChangeClientSecret;
public class HandlerTests
{
    [Fact]
    public async Task Change_client_secret()
    {
        // Arrange
        var client = new OAuthClient("some-client-id", string.Empty, TierId.Generate(), SystemTime.UtcNow, 1);

        var newClientSecret = "New-client-secret";
        var command = new ChangeClientSecretCommand(client.ClientId, newClientSecret);

        var oAuthClientsRepository = A.Fake<IOAuthClientsRepository>();
        A.CallTo(() => oAuthClientsRepository.Find(client.ClientId, A<CancellationToken>._, A<bool>._)).Returns(client);

        var handler = CreateHandler(oAuthClientsRepository);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        A.CallTo(() => oAuthClientsRepository.ChangeClientSecret(A<OAuthClient>.That.Matches(c =>
                c.ClientId == client.ClientId), newClientSecret
            , CancellationToken.None)
        ).MustHaveHappenedOnceExactly();
    }

    private Handler CreateHandler(IOAuthClientsRepository oAuthClientsRepository)
    {
        return new Handler(oAuthClientsRepository);
    }
}
