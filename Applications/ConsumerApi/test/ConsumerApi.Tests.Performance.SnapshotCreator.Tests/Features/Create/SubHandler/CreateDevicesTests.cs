using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.Factories;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.SubHandler;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;
using FakeItEasy;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Tests.Features.Create.SubHandler;

public class CreateDevicesTests
{
    private readonly IDeviceFactory _deviceFactory;
    private readonly CreateDevices.CommandHandler _sut;

    public CreateDevicesTests()
    {
        _deviceFactory = A.Fake<IDeviceFactory>();
        _sut = new CreateDevices.CommandHandler(_deviceFactory);
    }

    [Fact]
    public async Task Handle_ShouldReturnListOfDomainIdentities_WhenValidCommand()
    {
        // ARRANGE
        A.CallTo(() => _deviceFactory.Create(
                A<CreateDevices.Command>.Ignored,
                A<DomainIdentity>.Ignored))
            .Returns(Task.CompletedTask);

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
        await _sut.Handle(command, CancellationToken.None);

        // ASSERT
        A.CallTo(() => _deviceFactory.Create(
            A<CreateDevices.Command>.Ignored,
            A<DomainIdentity>.Ignored)).MustHaveHappened(domainIdentities.Count, Times.Exactly);

        _deviceFactory.TotalConfiguredDevices.Should().Be(sumOfExpectedDevices);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenCommandIsNull()
    {
        // ACT & ASSERT
        await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.Handle(null!, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoDevicesCreated()
    {
        // ARRANGE
        A.CallTo(() => _deviceFactory.Create(
                A<CreateDevices.Command>.Ignored,
                A<DomainIdentity>.Ignored))
            .Returns(Task.CompletedTask);

        var domainIdentity = A.Fake<DomainIdentity>();

        var domainIdentities = new List<DomainIdentity>
        {
            domainIdentity with { NumberOfDevices = 0 }
        };

        var command = new CreateDevices.Command(domainIdentities,
            "http://localhost:8081",
            new ClientCredentials("test", "test"));

        // ACT
        await _sut.Handle(command, CancellationToken.None);

        // ASSERT
        A.CallTo(() => _deviceFactory.Create(
            A<CreateDevices.Command>.Ignored,
            A<DomainIdentity>.Ignored)).MustHaveHappenedOnceExactly();

        _deviceFactory.TotalConfiguredDevices.Should().Be(0);
    }
}
