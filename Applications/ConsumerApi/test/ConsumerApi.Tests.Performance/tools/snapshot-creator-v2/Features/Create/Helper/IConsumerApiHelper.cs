using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.SubHandler;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.Helper;

public interface IConsumerApiHelper
{
    Task<string> OnBoardNewDevice(DomainIdentity identity, Client sdkClient);

    Task<Client> CreateForNewIdentity(CreateIdentities.Command request);
    Client CreateForExistingIdentity(CreateDevices.Command request, DomainIdentity identity);
}
