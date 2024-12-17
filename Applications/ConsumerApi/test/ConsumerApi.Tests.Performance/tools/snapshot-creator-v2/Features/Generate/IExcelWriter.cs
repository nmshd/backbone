using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Generate;

public interface IExcelWriter
{
    Task WritePoolConfigurations(string snapshotFolder, string worksheet, List<PoolConfiguration> poolConfigurations);
    Task WriteRelationshipsAndMessages(string snapshotFolder, string worksheet, List<RelationshipAndMessages> relationshipAndMessages);
}
