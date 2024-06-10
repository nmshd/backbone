using System;
using System.Text;
using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Sdk.Endpoints.Messages.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates.Types.Requests;
using Backbone.Crypto;
using Backbone.Identity.Pool.Creator.PoolsFile;
using Backbone.Identity.Pool.Creator.Tools;
using Backbone.Tooling;
using Backbone.Tooling.Extensions;

namespace Backbone.Identity.Pool.Creator.PoolsGenerator;
public class PoolsGenerator
{
    private readonly string _baseAddress;
    private readonly IList<PoolEntry> _pools;
    private readonly PoolsOffset _poolsOffset;
    private readonly ClientCredentials _clientCredentials;
    private readonly PoolFileConfiguration _poolsConfiguration;

    public PoolsGenerator(string baseAddress, string clientId, string clientSecret, PoolFileRoot configuration)
    {
        _baseAddress = baseAddress;
        _clientCredentials = new ClientCredentials(clientId, clientSecret);
        _pools = configuration.Pools.ToList();
        _poolsConfiguration = configuration.Configuration;
        CalculateSentAndReceivedMessages();
        _poolsOffset = PoolsOffset.CalculatePoolOffsets(_pools.ToArray());
    }

    private void CalculateSentAndReceivedMessages()
    {
        var messagesSentByConnectorRatio = _poolsConfiguration.MessagesSentByConnectorRatio;
        var messagesSentByAppRatio = 1 - messagesSentByConnectorRatio;

        foreach (var pool in _pools.Where(p => p.TotalNumberOfMessages > 0))
        {
            if (pool.IsApp())
            {
                pool.NumberOfReceivedMessages = Convert.ToUInt32(decimal.Ceiling(pool.TotalNumberOfMessages * messagesSentByAppRatio));
            }
            else if (pool.IsConnector())
            {
                pool.NumberOfReceivedMessages = Convert.ToUInt32(decimal.Ceiling(pool.TotalNumberOfMessages * messagesSentByConnectorRatio));
            }
            else
            {
                throw new Exception("Pools that are neither app nor connector cannot send messages.");
            }
            pool.NumberOfSentMessages = pool.TotalNumberOfMessages - pool.NumberOfReceivedMessages;
        }
    }

    public async Task CreatePools()
    {
        CreateOffsetPools();

        //await CreateIdentities();
        CreateFakeIdentities();
        DistributeRelationships();

        //await CreateRelationshipTemplates();
        //await CreateChallenges();
        //await CreateMessages();
        //await CreateDatawalletModifications();

        OutputAll();
    }

    private void CreateFakeIdentities()
    {
        foreach (var pool in _pools)
        {
            for (uint i = 0; i < pool.Amount; i++)
            {
                var createdIdentity = new Identity(new("USR" + PasswordHelper.GeneratePassword(8, 8), PasswordHelper.GeneratePassword(18, 24)), "ID1" + PasswordHelper.GeneratePassword(16, 16), "DVC" + PasswordHelper.GeneratePassword(8, 8), pool, i + 1);

                if (pool.NumberOfDevices > 1)
                {
                    for (uint j = 1; j < pool.NumberOfDevices; j++)
                        createdIdentity.AddDevice("DVC" + PasswordHelper.GeneratePassword(8, 8));
                }
                pool.Identities.Add(createdIdentity);
            }
        }
    }

    private void DistributeRelationships()
    {
        var appPools = _pools.Where(p => p.IsApp() && p.NumberOfRelationships > 0).ToList();
        var connectorPools = _pools.Where(p => p.IsConnector() && p.NumberOfRelationships > 0).ToList();

        var appRelationships = appPools.Sum(p => p.NumberOfRelationships * p.Amount);
        var connectorRelationships = connectorPools.Sum(p => p.NumberOfRelationships * p.Amount);

        if (appRelationships != connectorRelationships)
            throw new Exception(
                "The number of relationships in the app pools does not match the number of relationships in the connector pools, despite there being offset pools. There is an implementation error.");


        var targetIdentities = connectorPools.SelectMany(p => p.Identities).ToArray();
        var targetIdentitiesIteratorIndex = 0;

        while (appPools.Sum(p => p.Identities.Sum(i => i.IdentitiesToEstablishRelationshipsWith.Count)) < appRelationships)
        {
            foreach (var poolEntry in appPools)
            {
                foreach (var identity in poolEntry.Identities.Where(i => i.HasAvailabilityForNewRelationships()))
                {
                    Identity candidateIdentityForRelationship;
                    var counter = 0;
                    do
                    {
                        if (counter++ > targetIdentities.Length)
                        {
                            throw new Exception("The current algorithm cannot handle this input because the attribution of relationships is not trivial.");
                        }
                        candidateIdentityForRelationship = GetCandidateIdentityForRelationship(ref targetIdentities, ref targetIdentitiesIteratorIndex);

                    }
                    while (identity.IdentitiesToEstablishRelationshipsWith.Contains(candidateIdentityForRelationship));

                    identity.AddIdentityToEstablishRelationshipsWith(candidateIdentityForRelationship);
                }
            }
        }

        var relationshipsToA21 = appPools.SelectMany(p => p.Identities).SelectMany(i => i.IdentitiesToEstablishRelationshipsWith).Count(i => i.Nickname == "c21");
        var c21 = connectorPools.SelectMany(p => p.Identities).Where(i => i.Nickname == "c21").Single();
    }

    private static Identity GetCandidateIdentityForRelationship(ref Identity[] targetIdentities, ref int targetIdentitiesIteratorIndex)
    {
        var candidateIdentityForRelationship = targetIdentities[targetIdentitiesIteratorIndex];
        if (!candidateIdentityForRelationship.HasAvailabilityForNewRelationships())
        {
            // this identity has been exhausted and can be removed from the iteration list.
            targetIdentities = targetIdentities.Except([candidateIdentityForRelationship]).ToArray();
        }
        else
        {
            targetIdentitiesIteratorIndex++;
        }

        if (targetIdentitiesIteratorIndex == targetIdentities.Length - 1)
        {
            // removed the last item and fell out of the indexes. reset
            targetIdentitiesIteratorIndex = 0;
        }

        return candidateIdentityForRelationship;
    }


    #region Creators

    private async Task CreateMessages()
    {
        var connectorPools = _pools.Where(p => p.IsConnector()).ToList();
        var remainingPools = _pools.Except(connectorPools).ToList();

        foreach (var connectorPool in connectorPools)
        {
            foreach (var connectorPoolIdentity in connectorPool.Identities)
            {
                for (var i = 0; i < connectorPool.NumberOfSentMessages; i++)
                {
                    var sdk = Client.CreateForExistingIdentity(_baseAddress, _clientCredentials, connectorPoolIdentity.UserCredentials);
                    var candidateRecipientIdentity = remainingPools.SelectMany(p => p.Identities.Where(id => !id.HasBeenUsedAsMessageRecipient)).First();
                    candidateRecipientIdentity.UseAsMessageRecipient();
                    await sdk.Messages.SendMessage(new()
                    {
                        Recipients = [new SendMessageRequestRecipientInformation { Address = candidateRecipientIdentity.Address, EncryptedKey = ConvertibleString.FromUtf8(new string('A', 152)).BytesRepresentation }],
                        Attachments = [],
                        Body = []
                    });
                }
            }
        }
    }

    /// <summary>
    /// Creates identities pertaining to each pool.
    /// </summary>
    private async Task CreateIdentities()
    {
        Console.Write("Creating Identities... ");
        using var progress = new ProgressBar(_pools.Sum(p => p.Amount));

        foreach (var pool in _pools)
        {
            for (uint i = 0; i < pool.Amount; i++)
            {
                var sdk = await Client.CreateForNewIdentity(_baseAddress, _clientCredentials, PasswordHelper.GeneratePassword(18, 24));
                if (sdk.DeviceData is null)
                    throw new Exception("The SDK could not be used to create a new Identity.");

                var createdIdentity = new Identity(sdk.DeviceData.UserCredentials, sdk.IdentityData?.Address ?? "no address", sdk.DeviceData.DeviceId, pool, i + 1);

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

                pool.Identities.Add(createdIdentity);
                progress.Increment();
            }
        }
    }

    private async Task CreateRelationshipTemplates()
    {
        Console.Write("Creating RelationshipTemplates... ");
        using var progress = new ProgressBar(_pools.Sum(p => p.NumberOfRelationshipTemplates * p.Amount));
        foreach (var pool in _pools.Where(p => p.NumberOfRelationshipTemplates > 0))
        {
            foreach (var identity in pool.Identities)
            {
                var sdk = Client.CreateForExistingIdentity(_baseAddress, _clientCredentials, identity.UserCredentials);
                for (uint i = 0; i < pool.NumberOfRelationshipTemplates; i++)
                {
                    await sdk.RelationshipTemplates.CreateTemplate(new CreateRelationshipTemplateRequest
                    {
                        Content = [],
                        ExpiresAt = DateTime.Now.EndOfYear(),
                        MaxNumberOfAllocations = 10
                    });
                    progress.Increment();
                }
            }
        }
    }

    private async Task CreateChallenges()
    {
        Console.Write("Creating Challenges... ");
        using var progress = new ProgressBar(_pools.Sum(p => p.NumberOfChallenges * p.Amount));

        foreach (var pool in _pools.Where(p => p.NumberOfChallenges > 0))
        {
            foreach (var identity in pool.Identities)
            {
                var sdk = Client.CreateForExistingIdentity(_baseAddress, _clientCredentials, identity.UserCredentials);
                for (var i = 0; i < pool.NumberOfChallenges; i++)
                {
                    await sdk.Challenges.CreateChallenge();
                    progress.Increment();
                }
            }
        }
    }

    private async Task CreateDatawalletModifications()
    {
        Console.Write("Creating DataWalletModifications... ");
        using var progress = new ProgressBar(_pools.Sum(p => p.NumberOfDatawalletModifications * p.Amount));
        foreach (var pool in _pools.Where(p => p.NumberOfDatawalletModifications > 0))
        {
            foreach (var identity in pool.Identities)
            {
                var sdk = Client.CreateForExistingIdentity(_baseAddress, _clientCredentials, identity.UserCredentials);
                for (int i = 0; i < pool.NumberOfDatawalletModifications; i++)
                {
                    await sdk.Datawallet.PushDatawalletModifications(new()
                    {
                        LocalIndex = 0,
                        Modifications = []
                    }, 0);
                    progress.Increment();
                }
            }
        }
    }

    #endregion

    #region Outputters

    private void OutputAll()
    {
        var outputDirName = $@"{GetProjectPath()}\poolCreator.{SystemTime.UtcNow:yyyyMMdd-HHmmss}";
        Directory.CreateDirectory(outputDirName);

        OutputIdentities(outputDirName);
    }

    private void OutputIdentities(string outputDirName)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("DeviceId;Username;Password;Alias");
        foreach (var pool in _pools)
        {
            foreach (var identity in pool.Identities)
            {
                foreach (var deviceId in identity.DeviceIds)
                {
                    stringBuilder.AppendLine($"""{deviceId};{identity.UserCredentials.Username};"{identity.UserCredentials.Password}";{pool.Alias}""");
                }
            }
        }
        File.WriteAllTextAsync($@"{outputDirName}\identities.csv", stringBuilder.ToString());
    }

    private static string GetProjectPath()
    {
        var dir = Path.GetFullPath(@"..\..\..");
        return dir;
    }

    #endregion

    #region Offsets

    private void CreateOffsetPools()
    {
        // because the creation of identities is a somewhat heavy operation, we should avoid carelessly creating identities.
        // This requires finding the right ballance between identities and messages/relationships per identity.
        // This is a TODO

        var relationshipsOffsetPool = new PoolEntry
        {
            Name = $"{(_poolsOffset.RelationshipsOffsetPendingTo == OffsetDirections.App ? "App" : "Connector")} Offset Pool for Relationships",
            NumberOfDevices = 1,
            Amount = Convert.ToUInt32(_poolsOffset.RelationshipsOffset),
            Alias = _poolsOffset.RelationshipsOffsetPendingTo == OffsetDirections.App ? "a0r" : "c0r",
            NumberOfChallenges = 0,
            NumberOfDatawalletModifications = 0,
            NumberOfRelationshipTemplates = 0,
            NumberOfRelationships = 1,
            TotalNumberOfMessages = 0,
            Type = _poolsOffset.RelationshipsOffsetPendingTo == OffsetDirections.App ? "connector" : "app"
        };


        var messagesOffsetPool = new PoolEntry
        {
            Name = $"{(_poolsOffset.MessagesOffsetPendingTo == OffsetDirections.App ? "App" : "Connector")} Offset Pool for Messages",
            NumberOfDevices = 1,
            Amount = Convert.ToUInt32(_poolsOffset.MessagesOffset),
            Alias = _poolsOffset.MessagesOffsetPendingTo == OffsetDirections.App ? "a0m" : "c0m",
            NumberOfChallenges = 0,
            NumberOfDatawalletModifications = 0,
            NumberOfRelationshipTemplates = 0,
            NumberOfRelationships = 0,
            NumberOfSentMessages = 1,
            Type = _poolsOffset.MessagesOffsetPendingTo == OffsetDirections.App ? "connector" : "app"
        };

        if (_poolsOffset.RelationshipsOffset != 0)
            _pools.Add(relationshipsOffsetPool);

        if (_poolsOffset.MessagesOffset != 0)
            _pools.Add(messagesOffsetPool);
    }

    #endregion
}
