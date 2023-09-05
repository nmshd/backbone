using Backbone.Modules.Devices.Application.Clients.Commands.UpdateClient;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.OpenIddict;
using FakeItEasy;
using Xunit;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Clients.Commands.UpdateClient;
public class HandlerTests
{
    [Fact]
    public async Task Change_Client_Tier_Id()
    {
        // Arrange
        var client = new CustomOpenIddictEntityFrameworkCoreApplication
        {
            ClientId = "Some-client-id",
            ClientSecret = "Some-client-secret",
            TierId = "Old-tier-id"
        };

        var newTier = new Tier(TierName.Create("new-tier-name").Value);

        var command = new UpdateClientCommand(client.ClientId, newTier.Id);

        var oAuthClientsRepository = A.Fake<IOAuthClientsRepository>();
        A.CallTo(() => oAuthClientsRepository.Find(client.ClientId, A<CancellationToken>._)).Returns(client);

        var tiersRepository = A.Fake<ITiersRepository>();
        A.CallTo(() => tiersRepository.FindById(newTier.Id, A<CancellationToken>._)).Returns(newTier);

        var handler = CreateHandler(oAuthClientsRepository, tiersRepository);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        A.CallTo(() => oAuthClientsRepository.Update(A<CustomOpenIddictEntityFrameworkCoreApplication>.That.Matches(c =>
                c.ClientId == client.ClientId &&
                c.ClientSecret == client.ClientSecret &&
                c.TierId == newTier.Id)
            , CancellationToken.None)
        ).MustHaveHappenedOnceExactly();
    }

    private Handler CreateHandler(IOAuthClientsRepository oAuthClientsRepository, ITiersRepository tiersRepository)
    {
        return new Handler(oAuthClientsRepository, tiersRepository);
    }
}
