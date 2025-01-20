namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Readers.Base;

public abstract class PoolConfigurationReaderBase
{
    protected abstract string[] ValidExtensions { get; }

    protected void VerifyFileExtension(string filePath)
    {
        var fileExtension = Path.GetExtension(filePath);

        if (ValidExtensions.All(ext => ext != fileExtension))
        {
            throw new ArgumentException(message: string.Format(PERFORMANCE_TEST_CONFIG_READER_INVALID_FILE_EXT, string.Join(',', ValidExtensions), fileExtension));
        }
    }
}
