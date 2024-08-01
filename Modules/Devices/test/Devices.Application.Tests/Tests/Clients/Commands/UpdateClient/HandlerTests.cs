using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.Modules.Devices.Application.Clients.Commands.UpdateClient;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.Tests.Extensions;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities;
using Backbone.Tooling;
using Backbone.UnitTestTools.BaseClasses;
using FakeItEasy;
using FluentAssertions;
using Xunit;
using ApplicationException = Backbone.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Clients.Commands.UpdateClient;
public class HandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task Change_Default_Tier()
    {
        // Arrange
        var client = new OAuthClient("some-client-id", string.Empty, TierId.Generate(), SystemTime.UtcNow, 1);

        var newDefaultTier = new Tier(TierName.Create("new-default-tier").Value);

        var command = new UpdateClientCommand(client.ClientId, newDefaultTier.Id, 1);

        var oAuthClientsRepository = A.Fake<IOAuthClientsRepository>();
        A.CallTo(() => oAuthClientsRepository.Find(client.ClientId, A<CancellationToken>._, A<bool>._)).Returns(client);

        var identitiesRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => identitiesRepository.CountByClientId(client.ClientId, A<CancellationToken>._)).Returns(0);

        var tiersRepository = A.Fake<ITiersRepository>();
        A.CallTo(() => tiersRepository.ExistsWithId(newDefaultTier.Id, A<CancellationToken>._)).Returns(true);

        var handler = CreateHandler(oAuthClientsRepository, identitiesRepository, tiersRepository);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        A.CallTo(() => oAuthClientsRepository.Update(A<OAuthClient>.That.Matches(c =>
                c.ClientId == client.ClientId &&
                c.DefaultTier == newDefaultTier.Id)
            , CancellationToken.None)
        ).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Change_Default_Tier_With_Inexistent_Tier()
    {
        // Arrange
        var client = new OAuthClient("some-client-id", string.Empty, TierId.Generate(), SystemTime.UtcNow, 1);

        var newDefaultTier = new Tier(TierName.Create("new-default-tier").Value);

        var command = new UpdateClientCommand(client.ClientId, newDefaultTier.Id, 1);

        var oAuthClientsRepository = A.Fake<IOAuthClientsRepository>();
        A.CallTo(() => oAuthClientsRepository.Find(client.ClientId, A<CancellationToken>._, A<bool>._)).Returns(client);

        var identitiesRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => identitiesRepository.CountByClientId(client.ClientId, A<CancellationToken>._)).Returns(0);

        var tiersRepository = A.Fake<ITiersRepository>();
        A.CallTo(() => tiersRepository.ExistsWithId(newDefaultTier.Id, A<CancellationToken>._)).Returns(false);

        var handler = CreateHandler(oAuthClientsRepository, identitiesRepository, tiersRepository);

        // Act
        var acting = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await acting.Should().ThrowAsync<ApplicationException>().WithErrorCode("error.platform.validation.device.tierIdInvalidOrDoesNotExist");
    }

    [Fact]
    public async Task Change_Default_Tier_Of_Inexistent_Client()
    {
        // Arrange
        var client = new OAuthClient("some-client-id", string.Empty, TierId.Generate(), SystemTime.UtcNow, 1);

        var newDefaultTier = new Tier(TierName.Create("new-default-tier").Value);

        var command = new UpdateClientCommand(client.ClientId, newDefaultTier.Id, 1);

        var oAuthClientsRepository = A.Fake<IOAuthClientsRepository>();
        A.CallTo(() => oAuthClientsRepository.Find(client.ClientId, A<CancellationToken>._, A<bool>._)).Returns<OAuthClient?>(null);

        var identitiesRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => identitiesRepository.CountByClientId(client.ClientId, A<CancellationToken>._)).Returns(0);

        var tiersRepository = A.Fake<ITiersRepository>();
        A.CallTo(() => tiersRepository.ExistsWithId(newDefaultTier.Id, A<CancellationToken>._)).Returns(true);

        var handler = CreateHandler(oAuthClientsRepository, identitiesRepository, tiersRepository);

        // Act
        var acting = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await acting.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Change_Max_Identities()
    {
        // Arrange
        var client = new OAuthClient("some-client-id", string.Empty, TierId.Generate(), SystemTime.UtcNow, 1);

        var command = new UpdateClientCommand(client.ClientId, client.DefaultTier, 2);

        var oAuthClientsRepository = A.Fake<IOAuthClientsRepository>();
        A.CallTo(() => oAuthClientsRepository.Find(client.ClientId, A<CancellationToken>._, A<bool>._)).Returns(client);

        var identitiesRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => identitiesRepository.CountByClientId(client.ClientId, A<CancellationToken>._)).Returns(0);

        var tiersRepository = A.Fake<ITiersRepository>();
        A.CallTo(() => tiersRepository.ExistsWithId(client.DefaultTier, A<CancellationToken>._)).Returns(true);

        var handler = CreateHandler(oAuthClientsRepository, identitiesRepository, tiersRepository);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        A.CallTo(() => oAuthClientsRepository.Update(A<OAuthClient>.That.Matches(c =>
                c.ClientId == client.ClientId &&
                c.MaxIdentities == 2)
            , CancellationToken.None)
        ).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Change_Max_Identities_With_Value_Less_Than_Number_Of_Identities()
    {
        // Arrange
        var client = new OAuthClient("some-client-id", string.Empty, TierId.Generate(), SystemTime.UtcNow, 1);

        var command = new UpdateClientCommand(client.ClientId, client.DefaultTier, 2);

        var oAuthClientsRepository = A.Fake<IOAuthClientsRepository>();
        A.CallTo(() => oAuthClientsRepository.Find(client.ClientId, A<CancellationToken>._, A<bool>._)).Returns(client);

        var identitiesRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => identitiesRepository.CountByClientId(client.ClientId, A<CancellationToken>._)).Returns(5);

        var tiersRepository = A.Fake<ITiersRepository>();
        A.CallTo(() => tiersRepository.ExistsWithId(client.DefaultTier, A<CancellationToken>._)).Returns(true);

        var handler = CreateHandler(oAuthClientsRepository, identitiesRepository, tiersRepository);

        // Act
        var acting = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await acting.Should().ThrowAsync<ApplicationException>().WithErrorCode("error.platform.validation.device.maxIdentitiesLessThanCurrentIdentities");
    }

    [Fact]
    public async Task Change_Max_Identities_Of_Inexistent_Client()
    {
        // Arrange
        var client = new OAuthClient("some-client-id", string.Empty, TierId.Generate(), SystemTime.UtcNow, 1);

        var command = new UpdateClientCommand(client.ClientId, client.DefaultTier, 2);

        var oAuthClientsRepository = A.Fake<IOAuthClientsRepository>();
        A.CallTo(() => oAuthClientsRepository.Find(client.ClientId, A<CancellationToken>._, A<bool>._)).Returns<OAuthClient?>(null);

        var identitiesRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => identitiesRepository.CountByClientId(client.ClientId, A<CancellationToken>._)).Returns(0);

        var tiersRepository = A.Fake<ITiersRepository>();
        A.CallTo(() => tiersRepository.ExistsWithId(client.DefaultTier, A<CancellationToken>._)).Returns(true);

        var handler = CreateHandler(oAuthClientsRepository, identitiesRepository, tiersRepository);

        // Act
        var acting = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await acting.Should().ThrowAsync<NotFoundException>();
    }

    private static Handler CreateHandler(IOAuthClientsRepository oAuthClientsRepository, IIdentitiesRepository identitiesRepository, ITiersRepository tiersRepository)
    {
        return new Handler(oAuthClientsRepository, identitiesRepository, tiersRepository);
    }
}
