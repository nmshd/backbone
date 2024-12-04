using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.SubHandler;

public interface IDeviceFactory
{
    Task Create(CreateDevices.Command request, DomainIdentity identity);
    int TotalNumberOfDevices { get; set; }
    Task<string> OnBoardNewDevice(DomainIdentity identity, Client sdkClient);
}
