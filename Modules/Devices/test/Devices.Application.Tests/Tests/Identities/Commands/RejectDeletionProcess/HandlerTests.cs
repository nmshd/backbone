using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.BuildingBlocks.Domain;
using Backbone.Modules.Devices.Application.Identities.Commands.RejectDeletionProcess;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Tooling;
using Backbone.UnitTestTools.Extensions;
using FakeItEasy;
using FluentAssertions;
using Xunit;
using static Backbone.UnitTestTools.Data.TestDataGenerator;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Commands.RejectDeletionProcess;
public class HandlerTests
{
    [Fact]
    public async void Happy_path()
    {
        // Arrange
        var utcNow = SystemTime.UtcNow;
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
                && i.DeletionProcesses.Any(d => d.Id == deletionProcess.Id)), A<CancellationToken>._)) // todo: what if there are more than one rejected deletion process
            .MustHaveHappenedOnceExactly();

        response.Id.Should().Be(deletionProcess.Id);
        response.Status.Should().Be(DeletionProcessStatus.Rejected);
        response.CreatedAt.Should().Be(utcNow);
        response.RejectedByDevice.Should().Be(device.Id);
    }

    [Fact]
    public void Throws_when_given_identity_does_not_exist()
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
        acting.Should().AwaitThrowAsync<NotFoundException, RejectDeletionProcessResponse>().Which.Message.Should().Contain("Identity");
    }

    [Fact]
    public void Throws_when_deletion_process_does_not_exist()
    {
        // Arrange
        var utcNow = DateTime.Parse("2000-01-01");
        SystemTime.Set(utcNow);

        var identity = TestDataGenerator.CreateIdentityWithDeletionProcessWaitingForApproval(DateTime.Parse("2000-01-10"));
        var identityDevice = identity.Devices[0];

        var fakeUserContext = A.Fake<IUserContext>();
        A.CallTo(() => fakeUserContext.GetAddress()).Returns(identity.Address);
        A.CallTo(() => fakeUserContext.GetDeviceId()).Returns(identityDevice.Id);

        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => mockIdentitiesRepository.FindByAddress(identity.Address, A<CancellationToken>._, A<bool>._))
            .Returns(identity);

        var handler = CreateHandler(mockIdentitiesRepository, fakeUserContext);

        // Act
        var acting = async () => await handler.Handle(new RejectDeletionProcessCommand("IDP00000000000000001"), CancellationToken.None);

        // Assert
        acting.Should().AwaitThrowAsync<DomainException, RejectDeletionProcessResponse>().Which.Code.Should().Be("error.platform.recordNotFound");
    }

    [Fact]
    public void Throws_when_deletion_process_is_not_waiting_for_approval()
    {
        // Arrange
        var utcNow = DateTime.Parse("2000-01-01");
        SystemTime.Set(utcNow);

        var identity = TestDataGenerator.CreateIdentityWithApprovedDeletionProcess(DateTime.Parse("2000-01-10"));
        var identityDevice = identity.Devices[0];

        var fakeUserContext = A.Fake<IUserContext>();
        A.CallTo(() => fakeUserContext.GetAddress()).Returns(identity.Address);
        A.CallTo(() => fakeUserContext.GetDeviceId()).Returns(identityDevice.Id);

        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => mockIdentitiesRepository.FindByAddress(identity.Address, A<CancellationToken>._, A<bool>._))
            .Returns(identity);

        var handler = CreateHandler(mockIdentitiesRepository, fakeUserContext);

        // Act
        var acting = async () => await handler.Handle(new RejectDeletionProcessCommand(identity.DeletionProcesses.FirstOrDefault()!.Id), CancellationToken.None);

        // Assert
        acting.Should().AwaitThrowAsync<DomainException, RejectDeletionProcessResponse>().Which.Code.Should().Be("error.platform.validation.device.noDeletionProcessWithRequiredStatusExists");
    }

    private static Handler CreateHandler(IIdentitiesRepository identitiesRepository, IUserContext userContext)
    {
        return new Handler(identitiesRepository, userContext);
    }
}
