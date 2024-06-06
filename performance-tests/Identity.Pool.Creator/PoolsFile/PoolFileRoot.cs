using System.Text.Json.Serialization;

namespace Backbone.Identity.Pool.Creator.PoolsFile;
public class PoolFileRoot
{
    public PoolEntry[] Pools { get; set; }
    public PoolFileConfiguration Configuration { get; set; }
}

public class PoolFileConfiguration
{
    public decimal MessagesSentByConnectorRatio { get; set; }
}
