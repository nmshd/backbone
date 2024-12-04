using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.SubHandler;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;
using FakeItEasy;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Tests.Features.Create;

public class CreateDevicesTests
{
    [Fact]
    public async Task Handle_ShouldReturnListOfDomainIdentities_WhenValidCommand()
    {
        // ARRANGE
        var createDevicesCommand = A.Fake<IDeviceFactory>();

        A.CallTo(() => createDevicesCommand.Create(
                A<CreateDevices.Command>.Ignored,
                A<DomainIdentity>.Ignored))
            .Returns(Task.CompletedTask);


        var handler = new CreateDevices.CommandHandler(createDevicesCommand);

        var domainIdentity = A.Fake<DomainIdentity>();

        var domainIdentities = new List<DomainIdentity>
        {
            domainIdentity with { NumberOfDevices = 1 },
            domainIdentity with { NumberOfDevices = 2 },
            domainIdentity with { NumberOfDevices = 3 }
        };

        var sumOfExpectedDevices = domainIdentities.Sum(i => i.NumberOfDevices);

        var command = new CreateDevices.Command(domainIdentities,
            "http://localhost:8081",
            new ClientCredentials("test", "test"));


        // ACT
        await handler.Handle(command, CancellationToken.None);

        // ASSERT
        A.CallTo(() => createDevicesCommand.Create(
            A<CreateDevices.Command>.Ignored,
            A<DomainIdentity>.Ignored)).MustHaveHappened(domainIdentities.Count, Times.Exactly);

        createDevicesCommand.TotalNumberOfDevices.Should().Be(sumOfExpectedDevices);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenCommandIsNull()
    {
        // ARRANGE
        var deviceFactory = A.Fake<IDeviceFactory>();
        var handler = new CreateDevices.CommandHandler(deviceFactory);

        // ACT & ASSERT
        await Assert.ThrowsAsync<ArgumentNullException>(() => handler.Handle(null!, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoDevicesCreated()
    {
        // ARRANGE
        var deviceFactory = A.Fake<IDeviceFactory>();

        A.CallTo(() => deviceFactory.Create(
                A<CreateDevices.Command>.Ignored,
                A<DomainIdentity>.Ignored))
            .Returns(Task.CompletedTask);

        var handler = new CreateDevices.CommandHandler(deviceFactory);

        var domainIdentity = A.Fake<DomainIdentity>();

        var domainIdentities = new List<DomainIdentity>
        {
            domainIdentity with { NumberOfDevices = 0 }
        };

        var command = new CreateDevices.Command(domainIdentities,
            "http://localhost:8081",
            new ClientCredentials("test", "test"));

        // ACT
        await handler.Handle(command, CancellationToken.None);

        // ASSERT
        A.CallTo(() => deviceFactory.Create(
            A<CreateDevices.Command>.Ignored,
            A<DomainIdentity>.Ignored)).MustHaveHappenedOnceExactly();

        deviceFactory.TotalNumberOfDevices.Should().Be(0);
    }
}
