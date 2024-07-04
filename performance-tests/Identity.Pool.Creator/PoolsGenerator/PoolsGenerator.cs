using System.Text;
using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Sdk.Endpoints.Messages.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates.Types.Requests;
using Backbone.Crypto;
using Backbone.Identity.Pool.Creator.Application.MessageDistributor;
using Backbone.Identity.Pool.Creator.Application.Printer;
using Backbone.Identity.Pool.Creator.Application.RelationshipDistributor;
using Backbone.Identity.Pool.Creator.PoolsFile;
using Backbone.Identity.Pool.Creator.Tools;
using Backbone.Tooling;
using Backbone.Tooling.Extensions;

namespace Backbone.Identity.Pool.Creator.PoolsGenerator;
public class PoolsGenerator
{
    private readonly string _baseAddress;
    private readonly IRelationshipDistributor _relationshipDistributor;
    private readonly IMessageDistributor _messageDistributor;
    private readonly IPrinter _printer;
    private readonly IList<PoolEntry> _pools;
    private readonly PoolsOffset _poolsOffset;
    private readonly ClientCredentials _clientCredentials;

    public PoolsGenerator(
        string baseAddress,
        string clientId,
        string clientSecret,
        PoolFileRoot configuration,
        IRelationshipDistributor relationshipDistributor,
        IMessageDistributor messageDistributor,
        IPrinter printer)
    {
        _baseAddress = baseAddress;
        _clientCredentials = new ClientCredentials(clientId, clientSecret);
        _relationshipDistributor = relationshipDistributor;
        _messageDistributor = messageDistributor;
        _printer = printer;
        _pools = configuration.Pools.ToList();

        MessageDistributorTools.CalculateSentAndReceivedMessages(_pools, configuration.Configuration);
        _poolsOffset = PoolsOffset.CalculatePoolOffsets(_pools.ToArray());
    }

    public async Task CreatePools()
    {
        PoolsOffset.CreateOffsetPools(_pools, _poolsOffset);

        CheckPoolsConfiguration();

        //await CreateIdentities();
        CreateFakeIdentities();

        RelationshipDistributorTools.EstablishMessagesOffsetPoolsRelationships(_pools);

        _relationshipDistributor.Distribute(_pools);
        _printer.PrintRelationships(_pools, summaryOnly: true);
        _messageDistributor.Distribute(_pools);

        _printer.PrintRelationships(_pools, summaryOnly: true);
        Console.WriteLine($"Removed {TrimUnusedRelationships()} unused relationships.");

        _printer.PrintRelationships(_pools, summaryOnly: true);
        _printer.PrintMessages(_pools, summaryOnly: true);

        //await CreateRelationshipTemplates();
        //await CreateChallenges();
        //await CreateMessages();
        //await CreateDatawalletModifications();

        OutputAll();
    }

    private uint TrimUnusedRelationships()
    {
        var allIdentites = _pools.SelectMany(p => p.Identities);

        uint removedCount = 0;

        foreach (var identity in allIdentites)
        {
            for (var i = 0; i < identity.IdentitiesToEstablishRelationshipsWith.Count; i++)
            {
                var relationshipIdentity = identity.IdentitiesToEstablishRelationshipsWith[i];
                if (!identity.IdentitiesToSendMessagesTo.Contains(relationshipIdentity))
                {
                    identity.IdentitiesToEstablishRelationshipsWith.Remove(relationshipIdentity);
                    removedCount++;
                    i--;
                }
            }
        }

        return removedCount;
    }

    /// <summary>
    /// For each pool p, p.NumberOfRelationships must NOT be greater than the sum of identities in all pools of the opposite type
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void CheckPoolsConfiguration()
    {
        var expectedAppIdentitiesCount = _pools.Where(p => p.IsApp()).Sum(p => p.Amount);
        var expectedConnectorIdentitiesCount = _pools.Where(p => p.IsConnector()).Sum(p => p.Amount);

        foreach (var poolEntry in _pools)
        {
            var oppositeIdentitiesCount = poolEntry.IsApp() ? expectedConnectorIdentitiesCount : expectedAppIdentitiesCount;

            if (poolEntry.NumberOfRelationships > oppositeIdentitiesCount)
            {
                throw new Exception($"The number of relationships ({poolEntry.NumberOfRelationships}) for pool {poolEntry.Name} is higher than the number of identities in the opposite pools.");
            }
        }
    }

    private void CreateFakeIdentities()
    {
        uint uon = 0;
        foreach (var pool in _pools)
        {
            for (uint i = 0; i < pool.Amount; i++)
            {
                var createdIdentity = new Identity(new("USR" + PasswordHelper.GeneratePassword(8, 8), PasswordHelper.GeneratePassword(18, 24)), "ID1" + PasswordHelper.GeneratePassword(16, 16), "DVC" + PasswordHelper.GeneratePassword(8, 8), pool, uon++);

                if (pool.NumberOfDevices > 1)
                {
                    for (uint j = 1; j < pool.NumberOfDevices; j++)
                        createdIdentity.AddDevice("DVC" + PasswordHelper.GeneratePassword(8, 8));
                }
                pool.Identities.Add(createdIdentity);
            }
        }
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
                    //var candidateRecipientIdentity = remainingPools.SelectMany(p => p.Identities.Where(id => !id.HasBeenUsedAsMessageRecipient)).First();
                    //candidateRecipientIdentity.UseAsMessageRecipient();
                    //await sdk.Messages.SendMessage(new()
                    //{
                    //    Recipients = [new SendMessageRequestRecipientInformation { Address = candidateRecipientIdentity.Address, EncryptedKey = ConvertibleString.FromUtf8(new string('A', 152)).BytesRepresentation }],
                    //    Attachments = [],
                    //    Body = []
                    //});
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
                for (uint i = 0; i < pool.NumberOfDatawalletModifications; i++)
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
}
