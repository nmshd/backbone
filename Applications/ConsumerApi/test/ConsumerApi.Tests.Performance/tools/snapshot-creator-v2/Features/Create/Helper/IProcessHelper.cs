using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.Models;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.Helper;

public interface IProcessHelper
{
    Task<DatabaseRestoreResult> ExecuteProcess(string command, Predicate<ProcessParams> processPredicate);
}
