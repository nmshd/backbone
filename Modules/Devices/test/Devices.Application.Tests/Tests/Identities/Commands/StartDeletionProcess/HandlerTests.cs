using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Identities.Commands.StartDeletionProcess;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.UnitTestTools.Extensions;
using FakeItEasy;
using FluentAssertions;
using Xunit;
using static Backbone.UnitTestTools.Data.TestDataGenerator;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Commands.StartDeletionProcess;

public class HandlerTests
{
    [Fact]
    public async Task Happy_path_as_owner()
    {
        // Arrange
        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        var mockEventBus = A.Fake<IEventBus>();
        var identity = TestDataGenerator.CreateIdentityWithOneDevice();
        var fakeUserContext = A.Fake<IUserContext>();

        A.CallTo(() => mockIdentitiesRepository.FindByAddress(
                A<IdentityAddress>._, A<CancellationToken>._, A<bool>._))
            .Returns(identity);

        A.CallTo(() => fakeUserContext.GetAddressOrNull()).Returns(identity.Address);
        A.CallTo(() => fakeUserContext.GetDeviceId()).Returns(identity.Devices.First().Id);

        var handler = CreateHandler(mockIdentitiesRepository, mockEventBus, fakeUserContext);

        // Act
        var command = new StartDeletionProcessCommand();
        await handler.Handle(command, CancellationToken.None);

        // Assert
        A.CallTo(() => mockIdentitiesRepository.Update(
                A<Identity>.That.Matches(i => i.DeletionProcesses.Count == 1 && i.Address == identity.Address && i.DeletionProcesses.First().Status == DeletionProcessStatus.Approved),
                A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public void Cannot_start_when_given_identity_does_not_exist()
    {
        // Arrange
        var fakeIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        var fakeUserContext = A.Fake<IUserContext>();
        var address = CreateRandomIdentityAddress();

        A.CallTo(() => fakeIdentitiesRepository.FindByAddress(
                A<IdentityAddress>._,
                A<CancellationToken>._,
                A<bool>._))
            .Returns<Identity>(null);

        A.CallTo(() => fakeUserContext.GetAddressOrNull()).Returns(address);

        var handler = CreateHandler(fakeIdentitiesRepository, fakeUserContext);

        // Act
        var command = new StartDeletionProcessCommand();
        var acting = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        acting.Should().AwaitThrowAsync<NotFoundException>().Which.Message.Should().Contain("Identity");
    }

    private static Handler CreateHandler(IIdentitiesRepository identitiesRepository, IUserContext userContext)
    {
        return CreateHandler(identitiesRepository, A.Dummy<IEventBus>(), userContext);
    }

    private static Handler CreateHandler(IIdentitiesRepository identitiesRepository, IEventBus eventBus, IUserContext userContext)
    {
        return new Handler(identitiesRepository, eventBus, userContext);
    }
}
