namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Readers.Base;

public abstract class PerformanceTestConfigurationReaderBase
{
    protected abstract string[] ValidExtensions { get; }

    public void VerifyFileExtension(string filePath)
    {
        var fileExtension = Path.GetExtension(filePath);

        if (ValidExtensions.All(ext => ext != fileExtension))
        {
            throw new ArgumentException(message: string.Format(PERFORMANCE_TEST_CONFIG_READER_INVALID_FILE_EXT, string.Join(',', ValidExtensions), fileExtension));
        }
    }
}
