namespace Backbone.PerformanceSnapshotCreator.PoolsFile;

public class PoolFileRoot
{
    public required PoolEntry[] Pools { get; set; }
    public required PoolFileConfiguration Configuration { get; set; }
}

public class PoolFileConfiguration
{
    public decimal MessagesSentByConnectorRatio { get; set; }
}
