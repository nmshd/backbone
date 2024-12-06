using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.SubHandler;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.Factories;

public interface IRelationshipFactory
{
    Task Create(CreateRelationships.Command request, DomainIdentity appIdentity, DomainIdentity[] connectorIdentities);
    int TotalRelationships { get; set; }
}
