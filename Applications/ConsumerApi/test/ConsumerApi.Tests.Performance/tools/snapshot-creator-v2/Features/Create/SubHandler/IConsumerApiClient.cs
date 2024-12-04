using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.SubHandler;

public interface IConsumerApiClient
{
    Task<string> OnBoardNewDevice(DomainIdentity identity, Client sdkClient);
    Client CreateForExistingIdentity(CreateDevices.Command request, DomainIdentity identity);
}
