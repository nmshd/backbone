using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.SubHandler;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.Factories;

public interface IRelationshipTemplateFactory
{
    Task Create(CreateRelationshipTemplates.Command request, DomainIdentity identity);
    int TotalConfiguredRelationshipTemplates { get; set; }
}
