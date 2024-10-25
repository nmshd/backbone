using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Enums;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Models;

public record IdentityPool
{
    public IdentityPool(IdentityPoolConfiguration identityPoolConfiguration)
    {
        Alias = identityPoolConfiguration.Alias;
        Type = identityPoolConfiguration.Type switch
        {
            POOL_TYPE_NEVER => IdentityPoolType.Never,
            POOL_TYPE_APP => IdentityPoolType.App,
            POOL_TYPE_CONNECTOR => IdentityPoolType.Connector,
            _ => throw new InvalidOperationException(POOL_TYPE_UNKNOWN)
        };

        Identities = [];
        for (var i = 0; i < identityPoolConfiguration.Amount; i++)
        {
            Identities.Add(new Identity(i + 1, Type, identityPoolConfiguration));
        }
    }

    public string Alias { get; }

    public IdentityPoolType Type { get; }

    public List<Identity> Identities { get; }
}
