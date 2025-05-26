using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Devices.Application.Identities.Commands.RejectDeletionProcess;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Tooling;
using FakeItEasy;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Commands.RejectDeletionProcess;

public class HandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task Happy_path()
    {
        // Arrange
        var utcNow = DateTime.Parse("2000-01-01");
        SystemTime.Set(utcNow);

        var identity = TestDataGenerator.CreateIdentityWithDeletionProcessWaitingForApproval(utcNow);
        var deletionProcess = identity.GetDeletionProcessInStatus(DeletionProcessStatus.WaitingForApproval)!;
        var device = identity.Devices[0];

        var fakeUserContext = A.Fake<IUserContext>();
        A.CallTo(() => fakeUserContext.GetAddress()).Returns(identity.Address);
        A.CallTo(() => fakeUserContext.GetDeviceId()).Returns(device.Id);

        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => mockIdentitiesRepository.FindByAddress(identity.Address, A<CancellationToken>._, A<bool>._))
            .Returns(identity);

        var handler = CreateHandler(mockIdentitiesRepository, fakeUserContext);

        // Act
        var response = await handler.Handle(new RejectDeletionProcessCommand(deletionProcess.Id), CancellationToken.None);

        // Assert
        A.CallTo(() => mockIdentitiesRepository.Update(A<Identity>.That.Matches(i =>
                i.Address == identity.Address
                && i.Status == IdentityStatus.Active
                && i.DeletionProcesses.Any(d => d.Id == deletionProcess.Id)), A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();

        response.Id.ShouldBe(deletionProcess.Id);
        response.Status.ShouldBe(DeletionProcessStatus.Rejected);
    }

    [Fact]
    public async Task Throws_when_given_identity_does_not_exist()
    {
        // Arrange
        var address = CreateRandomIdentityAddress();
        var fakeIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        var fakeUserContext = A.Fake<IUserContext>();
        A.CallTo(() => fakeUserContext.GetAddress()).Returns(address);

        A.CallTo(() => fakeIdentitiesRepository.FindByAddress(address, A<CancellationToken>._, A<bool>._)).Returns<Identity?>(null);

        var handler = CreateHandler(fakeIdentitiesRepository, fakeUserContext);

        // Act
        var acting = async () => await handler.Handle(new RejectDeletionProcessCommand("some-deletion-process-id"), CancellationToken.None);

        // Assert
        var exception = await acting.ShouldThrowAsync<NotFoundException>();
        exception.Message.ShouldContain("Identity");
    }

    private static Handler CreateHandler(IIdentitiesRepository identitiesRepository, IUserContext userContext)
    {
        return new Handler(identitiesRepository, userContext);
    }
}
