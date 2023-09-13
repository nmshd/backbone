using Backbone.Modules.Devices.Application.Clients.Commands.UpdateClient;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities;
using FakeItEasy;
using Xunit;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Clients.Commands.UpdateClient;
public class HandlerTests
{
    [Fact]
    public async Task Change_Default_Tier()
    {
        // Arrange
        var client = new OAuthClient("some-client-id", string.Empty, TierId.Generate());

        var newDefaultTier = new Tier(TierName.Create("new-default-tier").Value);

        var command = new UpdateClientCommand(client.ClientId, newDefaultTier.Id);

        var oAuthClientsRepository = A.Fake<IOAuthClientsRepository>();
        A.CallTo(() => oAuthClientsRepository.Find(client.ClientId, A<CancellationToken>._, true)).Returns(client);

        var tiersRepository = A.Fake<ITiersRepository>();
        A.CallTo(() => tiersRepository.ExistsWithId(newDefaultTier.Id, A<CancellationToken>._)).Returns(true);

        var handler = CreateHandler(oAuthClientsRepository, tiersRepository);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        A.CallTo(() => oAuthClientsRepository.Update(A<OAuthClient>.That.Matches(c =>
                c.ClientId == client.ClientId &&
                c.DefaultTier == newDefaultTier.Id)
            , CancellationToken.None)
        ).MustHaveHappenedOnceExactly();
    }

    private Handler CreateHandler(IOAuthClientsRepository oAuthClientsRepository, ITiersRepository tiersRepository)
    {
        return new Handler(oAuthClientsRepository, tiersRepository);
    }
}
