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
    private readonly IList<PoolEntry> _pools;
    private readonly PoolsOffset _poolsOffset;

    public PoolsGenerator(string baseAddress, string clientId, string clientSecret, IEnumerable<PoolEntry> pools)
    {
        _baseAddress = baseAddress;
        _clientId = clientId;
        _clientSecret = clientSecret;
        _pools = pools.ToList();
        _poolsOffset = PoolsOffset.CalculatePoolOffsets(_pools.ToArray());
    }

    public async Task CreatePools()
    {
        CreateOffsetPools();
        await CreateIdentities();

        OutputAll();
    }

    private void OutputAll()
    {
        foreach (var pool in _pools)
        {
            foreach (var identity in pool.identities)
            {
                foreach (var deviceId in identity.DeviceIds)
                {
                    Console.WriteLine($"{deviceId}, {identity.UserCredentials.Username}, {identity.UserCredentials.Password}, {pool.Alias}");
                }
            }
        }
    }

    private void CreateOffsetPools()
    {
        var relationshipsOffsetPool = new PoolEntry
        {
            Name = $"{(_poolsOffset.RelationshipsOffsetPendingTo == OffsetDirections.App ? "App" : "Connector")} Offset Pool for Relationships",
            NumberOfDevices = 1,
            Amount = 1,
            Alias = _poolsOffset.RelationshipsOffsetPendingTo == OffsetDirections.App ? "a0r" : "c0r",
            NumberOfChallenges = 0,
            NumberOfDatawalletModifications = 0,
            NumberOfRelationshipTemplates = 0,
            NumberOfRelationships = Convert.ToUInt32(_poolsOffset.RelationshipsOffset),
            NumberOfSentMessages = 0,
            Type = "Offset"
        };

        var messagesOffsetPool = new PoolEntry
        {
            Name = $"{(_poolsOffset.RelationshipsOffsetPendingTo == OffsetDirections.App ? "App" : "Connector")} Offset Pool for Messages",
            NumberOfDevices = 1,
            Amount = 1,
            Alias = _poolsOffset.RelationshipsOffsetPendingTo == OffsetDirections.App ? "a0m" : "c0m",
            NumberOfChallenges = 0,
            NumberOfDatawalletModifications = 0,
            NumberOfRelationshipTemplates = 0,
            NumberOfRelationships = 0,
            NumberOfSentMessages = Convert.ToUInt32(_poolsOffset.MessagesOffset),
            Type = "Offset"
        };

        if (_poolsOffset.RelationshipsOffset != 0)
            _pools.Add(relationshipsOffsetPool);

        if (_poolsOffset.MessagesOffset != 0)
            _pools.Add(messagesOffsetPool);
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
                var sdk = await Client.CreateForNewIdentity(_baseAddress, new ClientCredentials(_clientId, _clientSecret), PasswordHelper.GeneratePassword(18, 24));
                if (sdk.DeviceData is null)
                    throw new Exception("The SDK could not be used to create a new Identity.");

                var createdIdentity = new Identity(sdk.DeviceData.UserCredentials, sdk.DeviceData.DeviceId);

                if (pool.NumberOfDevices > 1)
                {
                    for (uint j = 1; j < pool.NumberOfDevices; j++)
                    {
                        var newDevice = await sdk.OnboardNewDevice(PasswordHelper.GeneratePassword(18, 24));
                        if (newDevice.DeviceData is null)
                            throw new Exception("The SDK could not be used to create a new Identity.");
                        createdIdentity.AddDevice(newDevice.DeviceData.DeviceId);
                    }
                }

                pool.identities.Add(createdIdentity);
            }
        }
    }
}
