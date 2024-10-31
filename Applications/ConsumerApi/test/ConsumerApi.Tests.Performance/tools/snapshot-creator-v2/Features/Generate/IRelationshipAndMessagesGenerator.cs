using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Models;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Generate;

public interface IRelationshipAndMessagesGenerator
{
    RelationshipAndMessages[] Generate(PerformanceTestConfiguration performanceTestConfiguration);
}
