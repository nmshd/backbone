using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.Modules.Devices.Application.Identities.Commands.StartDeletionProcessAsSupport;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.UnitTestTools.BaseClasses;
using Backbone.UnitTestTools.Extensions;
using FakeItEasy;
using FluentAssertions;
using Xunit;
using static Backbone.UnitTestTools.Data.TestDataGenerator;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Commands.StartDeletionProcessAsSupport;

public class HandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task Happy_path()
    {
        // Arrange
        var activeIdentity = TestDataGenerator.CreateIdentityWithOneDevice();

        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();

        A.CallTo(() => mockIdentitiesRepository.FindByAddress(activeIdentity.Address, A<CancellationToken>._, A<bool>._))
            .Returns(activeIdentity);

        var handler = CreateHandler(mockIdentitiesRepository);

        // Act
        var response = await handler.Handle(new StartDeletionProcessAsSupportCommand(activeIdentity.Address), CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.Status.Should().Be(DeletionProcessStatus.WaitingForApproval);

        A.CallTo(() => mockIdentitiesRepository.Update(
                A<Identity>.That.Matches(
                    i => i.Address == activeIdentity.Address &&
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
        var address = CreateRandomIdentityAddress();

        var fakeIdentitiesRepository = A.Fake<IIdentitiesRepository>();

        A.CallTo(() => fakeIdentitiesRepository.FindByAddress(
                address,
                A<CancellationToken>._,
                A<bool>._))
            .Returns<Identity?>(null);

        var handler = CreateHandler(fakeIdentitiesRepository);

        // Act
        var acting = async () => await handler.Handle(new StartDeletionProcessAsSupportCommand(address), CancellationToken.None);

        // Assert
        acting.Should().AwaitThrowAsync<NotFoundException, StartDeletionProcessAsSupportResponse>().Which.Message.Should().Contain("Identity");
    }

    private static Handler CreateHandler(IIdentitiesRepository identitiesRepository)
    {
        return new Handler(identitiesRepository);
    }
}
