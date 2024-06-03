using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.Identity.Pool.Creator.PoolsFile;
using Backbone.Tooling;

namespace Backbone.Identity.Pool.Creator.PoolsGenerator;
public class PoolsGenerator
{
    private readonly string _baseAddress;
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly IEnumerable<PoolEntry> _pools;
    private readonly PoolsOffset _poolsOffset;

    public PoolsGenerator(string baseAddress, string clientId, string clientSecret, IEnumerable<PoolEntry> pools)
    {
        _baseAddress = baseAddress;
        _clientId = clientId;
        _clientSecret = clientSecret;
        _pools = pools;
        _poolsOffset = PoolsOffset.CalculatePoolOffsets(pools.ToArray());
    }

    public async Task CreatePools()
    {
        await CreateIdentities();
    }


    /// <summary>
    /// Creates identities pertaining to each pool.
    /// </summary>
    private async Task CreateIdentities()
    {
        foreach (var pool in _pools)
        {
            for (uint i = 0; i < pool.Amount; i++)
            {
                var password = PasswordHelper.GeneratePassword(18, 24);
                var sdk = await Client.CreateForNewIdentity(_baseAddress, new ClientCredentials(_clientId, _clientSecret), password);
                if (sdk.DeviceData is null)
                    throw new Exception("The SDK could not be used to create a new Identity.");

                var createdIdentity = new Identity(sdk.DeviceData.UserCredentials, sdk.DeviceData.DeviceId);

                if (pool.NumberOfDevices > 1)
                {
                    for (uint j = 2; j < pool.NumberOfDevices; j++)
                    {
                        var sdk2 = Client.CreateForExistingIdentity(_baseAddress, new ClientCredentials(_clientId, _clientSecret), createdIdentity.UserCredentials);
                        if (sdk2.DeviceData is null)
                            throw new Exception("The SDK could not be used to create a new Identity.");
                        createdIdentity.AddDevice(sdk2.DeviceData.DeviceId);
                    }
                }

                pool.identities.Add(createdIdentity);
            }
        }
    }
}
