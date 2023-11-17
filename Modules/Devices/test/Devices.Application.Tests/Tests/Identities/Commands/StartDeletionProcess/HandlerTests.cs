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
        var identity = TestDataGenerator.CreateIdentityWithOneDevice();
        var activeDevice = identity.Devices[0];
        var fakeUserContext = A.Fake<IUserContext>();

        A.CallTo(() => mockIdentitiesRepository.FindByAddress(A<IdentityAddress>._, A<CancellationToken>._, A<bool>._))
            .Returns(identity);

        A.CallTo(() => fakeUserContext.GetAddressOrNull()).Returns(identity.Address);
        A.CallTo(() => fakeUserContext.GetDeviceId()).Returns(activeDevice.Id);

        var handler = CreateHandler(mockIdentitiesRepository, fakeUserContext);

        // Act
        var command = new StartDeletionProcessCommand();
        var response = await handler.Handle(command, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.ApprovedByDevice.Should().NotBeNull();

        A.CallTo(() => mockIdentitiesRepository.Update(
                A<Identity>.That.Matches(
                    i => i.Address == identity.Address &&
                         i.DeletionProcesses.Count == 1 &&
                         i.DeletionProcesses[0].Id == response.Id &&
                         i.DeletionProcesses[0].AuditLog.Count == 1),
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
        acting.Should().AwaitThrowAsync<NotFoundException, StartDeletionProcessResponse>().Which.Message.Should().Contain("Identity");
    }

    private static Handler CreateHandler(IIdentitiesRepository identitiesRepository, IUserContext userContext)
    {
        return new Handler(identitiesRepository, userContext);
    }
}
