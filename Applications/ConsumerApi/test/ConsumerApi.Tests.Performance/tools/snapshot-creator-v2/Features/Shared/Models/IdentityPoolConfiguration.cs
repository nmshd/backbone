using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Enums;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;

public record IdentityPoolConfiguration
{
    public IdentityPoolConfiguration(PoolConfiguration poolConfiguration)
    {
        Alias = poolConfiguration.Alias;
        Type = poolConfiguration.Type switch
        {
            POOL_TYPE_NEVER => IdentityPoolType.Never,
            POOL_TYPE_APP => IdentityPoolType.App,
            POOL_TYPE_CONNECTOR => IdentityPoolType.Connector,
            _ => throw new InvalidOperationException(POOL_TYPE_UNKNOWN)
        };

        Identities = [];
        for (var i = 0; i < poolConfiguration.Amount; i++)
        {
            Identities.Add(new IdentityConfiguration(i + 1, Type, poolConfiguration));
        }
    }

    public string Alias { get; }

    public IdentityPoolType Type { get; }

    public List<IdentityConfiguration> Identities { get; }
}
