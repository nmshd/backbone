using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.Modules.Devices.Application.Identities.Commands.StartDeletionProcessAsSupport;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.UnitTestTools.Extensions;
using FakeItEasy;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Commands.StartDeletionProcessAsSupport;

public class HandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task Happy_path()
    {
        // Arrange
        var activeIdentity = TestDataGenerator.CreateIdentityWithOneDevice();

        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();

        A.CallTo(() => mockIdentitiesRepository.Get(activeIdentity.Address, A<CancellationToken>._, A<bool>._))
            .Returns(activeIdentity);

        var handler = CreateHandler(mockIdentitiesRepository);

        // Act
        var response = await handler.Handle(new StartDeletionProcessAsSupportCommand(activeIdentity.Address), CancellationToken.None);

        // Assert
        response.ShouldNotBeNull();
        response.Status.ShouldBe(DeletionProcessStatus.WaitingForApproval);

        A.CallTo(() => mockIdentitiesRepository.Update(
                A<Identity>.That.Matches(i => i.Address == activeIdentity.Address &&
                                              i.DeletionProcesses.Count == 1 &&
                                              i.DeletionProcesses[0].Id == response.Id &&
                                              i.DeletionProcesses[0].AuditLog.Count == 1),
                A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Cannot_start_when_given_identity_does_not_exist()
    {
        // Arrange
        var address = CreateRandomIdentityAddress();

        var fakeIdentitiesRepository = A.Fake<IIdentitiesRepository>();

        A.CallTo(() => fakeIdentitiesRepository.Get(
                address,
                A<CancellationToken>._,
                A<bool>._))
            .Returns<Identity?>(null);

        var handler = CreateHandler(fakeIdentitiesRepository);

        // Act
        var acting = async () => await handler.Handle(new StartDeletionProcessAsSupportCommand(address), CancellationToken.None);

        // Assert
        var exception = await acting.ShouldThrowAsync<NotFoundException>();
        exception.Message.ShouldContain("Identity");
    }

    private static Handler CreateHandler(IIdentitiesRepository identitiesRepository)
    {
        return new Handler(identitiesRepository);
    }
}
