using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.SubHandler;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.Factories;

public interface IDatawalletModificationFactory
{
    Task Create(CreateDatawalletModifications.Command request, DomainIdentity identity);
    int TotalConfiguredDatawalletModifications { get; set; }
}
