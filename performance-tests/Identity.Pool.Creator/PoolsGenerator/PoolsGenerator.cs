﻿using System.Text;
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

    public PoolsGenerator(string baseAddress,
        string clientId,
        string clientSecret,
        PoolFileRoot configuration,
        IRelationshipDistributor relationshipDistributor,
        IMessageDistributor messageDistributor,
        IPrinter printer)
    {
        _baseAddress = baseAddress;
        _relationshipDistributor = relationshipDistributor;
        _messageDistributor = messageDistributor;
        _printer = printer;
        _clientCredentials = new ClientCredentials(clientId, clientSecret);
        _pools = configuration.Pools.ToList();
        
        MessageDistributorTools.CalculateSentAndReceivedMessages(_pools, configuration.Configuration);
        _poolsOffset = PoolsOffset.CalculatePoolOffsets(_pools.ToArray());
    }

    public async Task CreatePools()
    {
        CreateOffsetPools();

        // TODO add check: for each pool p, p.NumberOfRelationships must NOT be greater than the number of identities it its pool's opposite type

        //await CreateIdentities();
        CreateFakeIdentities();

        RelationshipDistributorTools.EstablishMessagesOffsetPoolsRelationships(_pools);
        
        _relationshipDistributor.Distribute(_pools);
        _printer.PrintRelationships(_pools, summaryOnly: true);

        _messageDistributor.Distribute(_pools);
        _printer.PrintMessages(_pools, summaryOnly: true);

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

    #region Offsets

    private void CreateOffsetPools()
    {
        if (_poolsOffset.RelationshipsOffset != 0)
        {
            var avgCeiling = Convert.ToUInt32(Math.Ceiling(_pools.Where(p => p.NumberOfRelationships > 0).Average(p => p.NumberOfRelationships)));
            var otherPoolsRelationshipsAverage = avgCeiling % 2 == 0 ? avgCeiling : avgCeiling - 1;
            if (_poolsOffset.RelationshipsOffset < otherPoolsRelationshipsAverage / 2)
            {
                otherPoolsRelationshipsAverage = Convert.ToUInt32(_poolsOffset.RelationshipsOffset / 10);
            }

            _pools.Add(new PoolEntry
            {
                Name = $"{(_poolsOffset.RelationshipsOffsetPendingTo == OffsetDirections.App ? "Connector" : "App")} Offset Pool for Relationships",
                NumberOfDevices = 1,
                Amount = Convert.ToUInt32(_poolsOffset.RelationshipsOffset / otherPoolsRelationshipsAverage),
                Alias = _poolsOffset.RelationshipsOffsetPendingTo == OffsetDirections.App ? "c0r" : "a0r",
                NumberOfRelationships = otherPoolsRelationshipsAverage,
                Type = _poolsOffset.RelationshipsOffsetPendingTo == OffsetDirections.App ? "connector" : "app"
            });
        }

        if (_poolsOffset.MessagesOffset != 0)
        {
            var messagesOffsetPool1 = new PoolEntry
            {
                Name = $"{(_poolsOffset.MessagesOffsetPendingTo == OffsetDirections.App ? "Connector" : "App")} Offset Pool for Messages",
                NumberOfDevices = 1,
                Amount = 1,
                Alias = _poolsOffset.MessagesOffsetPendingTo == OffsetDirections.App ? "c0m" : "a0m",
                NumberOfRelationships = 1,
                TotalNumberOfMessages = Convert.ToUInt32(_poolsOffset.MessagesOffset),
                Type = _poolsOffset.MessagesOffsetPendingTo == OffsetDirections.App ? "connector" : "app"
            };

            // this pool is created simply to balance the 1 relationship created by the Pool above.
            var messagesOffsetPool2 = new PoolEntry
            {
                Name = $"{(_poolsOffset.MessagesOffsetPendingTo == OffsetDirections.App ? "App" : "Connector")} Compensation Offset Pool for Messages",
                NumberOfDevices = 1,
                Amount = 1,
                Alias = _poolsOffset.MessagesOffsetPendingTo == OffsetDirections.App ? "a0mc" : "c0mc",
                NumberOfRelationships = 1,
                Type = _poolsOffset.MessagesOffsetPendingTo == OffsetDirections.App ? "app" : "connector"
            };

            _pools.Add(messagesOffsetPool1);
            _pools.Add(messagesOffsetPool2);
        }
    }

    #endregion
}
