using Backbone.Modules.Devices.Application.Clients.Commands.ChangeClientSecret;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using FakeItEasy;
using OpenIddict.EntityFrameworkCore.Models;
using Xunit;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Clients.ChangeClientSecret;
public class HandlerTests
{
    [Fact]
    public async Task Change_client_secret()
    {
        // Arrange
        var client = new OpenIddictEntityFrameworkCoreApplication
        {
            ClientId = "Some-client-id",
            ClientSecret = "Old-client-secret"
        };
        var newClientSecret = "New-client-secret";
        var command = new ChangeClientSecretCommand(client.ClientId, newClientSecret);

        var oAuthClientsRepository = A.Fake<IOAuthClientsRepository>();
        A.CallTo(() => oAuthClientsRepository.Find(client.ClientId, A<CancellationToken>._)).Returns(client);

        var handler = CreateHandler(oAuthClientsRepository);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        A.CallTo(() => oAuthClientsRepository.Update(A<OpenIddictEntityFrameworkCoreApplication>.That.Matches(c =>
                c.ClientId == client.ClientId &&
                c.ClientSecret == newClientSecret)
            , CancellationToken.None)
        ).MustHaveHappenedOnceExactly();
    }

    private Handler CreateHandler(IOAuthClientsRepository oAuthClientsRepository)
    {
        return new Handler(oAuthClientsRepository);
    }
}
