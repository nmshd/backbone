namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Generate;

public interface IExcelWriter
{
    Task Write<T>(string filePath, IEnumerable<T> data);
}
