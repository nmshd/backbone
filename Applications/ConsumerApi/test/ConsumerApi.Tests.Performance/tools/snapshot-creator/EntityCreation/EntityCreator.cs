using System.Collections.Concurrent;
using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Sdk.Endpoints.Challenges.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Datawallets.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.Messages.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.Relationships.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.SyncRuns.Types.Requests;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Application.Printer;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Domain;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.PoolsFile;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.PoolsGenerator;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Tools;
using Backbone.Crypto;
using Backbone.Tooling;
using Backbone.Tooling.Extensions;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.EntityCreation;

public class EntityCreator
{
    private readonly IList<PoolEntry> _pools;
    private readonly SolutionRepresentation _ram;
    private readonly IPrinter _printer;
    private readonly Dictionary<int, HttpClient> _httpClientPool;
    private readonly ClientCredentials _clientCredentials;

    public EntityCreator(string baseAddress, string clientId, string clientSecret, IEnumerable<PoolEntry> pools, SolutionRepresentation ram, IPrinter printer)
    {
        _clientCredentials = new ClientCredentials(clientId, clientSecret);
        _pools = pools.ToList();
        _ram = ram;
        _printer = printer;
        _httpClientPool = Enumerable.Range(0, Environment.ProcessorCount * 4).ToDictionary(i => i, _ => new HttpClient { BaseAddress = new Uri(baseAddress) });
    }

    public async Task StartCreation()
    {
        CreateCompensationPool();

        await CreateIdentities();
        LoadRelationshipsAndMessagesConfiguration();
        _printer.OutputAll(_pools, PrintTarget.Identities);

        await CreateRelationshipTemplates();
        _printer.OutputAll(_pools, PrintTarget.RelationshipTemplates);

        await CreateRelationships();
        _printer.OutputAll(_pools, PrintTarget.Relationships);

        await CreateChallenges();
        _printer.OutputAll(_pools, PrintTarget.Challenges);

        await CreateMessages();
        _printer.OutputAll(_pools, PrintTarget.Messages);

        await CreateDatawalletModifications();
        _printer.OutputAll(_pools, PrintTarget.DatawalletModifications);
    }

    private void CreateCompensationPool()
    {
        var diff = _ram.GetIdentityCount() - _pools.Sum(p => p.Amount);
        if (diff <= 0) return;

        _pools.Add(new PoolEntry
        {
            Alias = "ac",
            Name = "App Compensation",
            NumberOfRelationships = 5,
            Type = "app",
            Amount = Convert.ToUInt32(diff + 1)
        });
    }

    /// <summary>
    /// Creates relationships. Despite being considered undirected, they must be created by an identity (a), using another identity's (b) relationship template.
    /// This means that the creation of relationship templates must precede the creation of relationships.
    /// Moreover, templates are created exclusively by connectors, meaning that relationships are initiated by app identities.
    /// </summary>
    /// <returns></returns>
    private async Task CreateRelationships()
    {
        // ensure all connectors involved have at least one relationship template
        var connectorIdentities = _pools.SelectMany(p => p.Identities).SelectMany(i => i.IdentitiesToEstablishRelationshipsWith).Distinct().Where(i => i.Pool.IsConnector());
        var nonConnectorIdentities = _pools.SelectMany(p => p.Identities).SelectMany(i => i.IdentitiesToEstablishRelationshipsWith).Distinct().Where(i => !i.Pool.IsConnector());
        if (connectorIdentities.Any(c => c.RelationshipTemplates.Count < 1))
            throw new Exception("One or more relationship target connectors do not have a usable relationship template.");

        Console.Write("Establishing Relationships... ");
        using var progress = new ProgressBar(_ram.GetRelationshipCount());


        await Parallel.ForEachAsync(nonConnectorIdentities, new ParallelOptions { MaxDegreeOfParallelism = _pools.ExpectedNumberOfRelationships() < 32 ? 4 : 16 }, async (nonConnectorIdentity, _) =>
        {
            var random = new Random();
            foreach (var relatedIdentity in nonConnectorIdentity.IdentitiesToEstablishRelationshipsWith)
            {
                var sdk = Client.CreateForExistingIdentity(_httpClientPool[Environment.CurrentManagedThreadId], _clientCredentials, nonConnectorIdentity.UserCredentials);
                var connectorSdk = Client.CreateForExistingIdentity(_httpClientPool[Environment.CurrentManagedThreadId + Environment.ProcessorCount], _clientCredentials,
                    relatedIdentity.UserCredentials);
                var randomRelationshipTemplate = random.GetRandomElement(relatedIdentity.RelationshipTemplates);

                var createRelationshipResponse = await sdk.Relationships.CreateRelationship(new CreateRelationshipRequest { RelationshipTemplateId = randomRelationshipTemplate.Id, Content = [] });
                var acceptRelationshipResponse = await connectorSdk.Relationships.AcceptRelationship(createRelationshipResponse.Result!.Id, new AcceptRelationshipRequest());

                if (acceptRelationshipResponse.Result is not null)
                    nonConnectorIdentity.EstablishedRelationshipsById.Add(acceptRelationshipResponse.Result.Id, relatedIdentity);

                progress.Increment();
            }
        });

        Console.WriteLine("done.");
    }

    private void LoadRelationshipsAndMessagesConfiguration()
    {
        var dict = _pools.SelectMany(p => p.Identities).ToDictionary(i => i.UniqueOrderNumber);

        foreach (var pool in _pools)
        {
            foreach (var identity in pool.Identities)
            {
                var res = _ram.GetRelationshipsAndMessageSentCountByIdentity(identity.UniqueOrderNumber);
                foreach (var (relatedIdentity, messageCount) in res)
                {
                    identity.AddIdentityToEstablishRelationshipsWith(dict[relatedIdentity], skipCapacityCheck: true);
                    for (var i = 0; i < messageCount; i++)
                    {
                        identity.SendMessageTo(dict[relatedIdentity], true);
                    }
                }
            }
        }
    }

    private async Task CreateMessages()
    {
        Console.Write("Sending Messages... ");
        using var progress = new ProgressBar(_ram.GetSentMessagesCount());

        foreach (var identity in _pools.SelectMany(p => p.Identities))
        {
            foreach (var recipientIdentity in identity.IdentitiesToSendMessagesTo)
            {
                var sdk = Client.CreateForExistingIdentity(_httpClientPool[0], _clientCredentials, identity.UserCredentials);

                var messageResponse = await sdk.Messages.SendMessage(new SendMessageRequest
                {
                    Recipients =
                    [
                        new SendMessageRequestRecipientInformation { Address = recipientIdentity.Address, EncryptedKey = ConvertibleString.FromUtf8(new string('A', 152)).BytesRepresentation }
                    ],
                    Attachments = [],
                    Body = ConvertibleString.FromUtf8("Message body").BytesRepresentation
                });
                if (messageResponse.Result is not null)
                    identity.SentMessagesIdRecipientPair.Add((messageResponse.Result.Id, recipientIdentity));
                progress.Increment();
            }
        }

        Console.WriteLine("done.");
    }

    /// <summary>
    /// Creates identities pertaining to each pool.
    /// </summary>
    private async Task CreateIdentities()
    {
        Console.Write("Creating Identities... ");
        using var progress = new ProgressBar(_pools.Sum(p => p.Amount));

        uint uon = 0;

        foreach (var pool in _pools)
        {
            for (uint i = 0; i < pool.Amount; i++)
            {
                pool.IdentityUniqueOrderNumbers.Enqueue(uon++);
            }
        }

        await Parallel.ForEachAsync(_pools, async (pool, _) =>
        {
            for (uint i = 0; i < pool.Amount; i++)
            {
                var sdk = await Client.CreateForNewIdentity(_httpClientPool[Environment.CurrentManagedThreadId], _clientCredentials, PasswordHelper.GeneratePassword(18, 24));
                if (sdk.DeviceData is null)
                    throw new Exception("The SDK could not be used to create a new Identity.");

                var createdIdentity = new Identity(sdk.DeviceData.UserCredentials, sdk.IdentityData?.Address ?? "no address", sdk.DeviceData.DeviceId, pool, i + 1);

                if (pool.NumberOfDevices > 1)
                {
                    for (uint j = 1; j < pool.NumberOfDevices; j++)
                    {
                        var newDevice = await sdk.OnboardNewDevice(PasswordHelper.GeneratePassword(18, 24));
                        if (newDevice.DeviceData is null)
                            throw new Exception("The SDK could not be used to create a new Device.");
                        createdIdentity.AddDevice(newDevice.DeviceData.DeviceId);
                    }
                }

                pool.Identities.Add(createdIdentity);
                progress.Increment();
            }
        });
        Console.WriteLine("done.");
    }

    private async Task CreateRelationshipTemplates()
    {
        Console.Write("Creating RelationshipTemplates... ");
        using var progress = new ProgressBar(_pools.Sum(p => p.NumberOfRelationshipTemplates * p.Amount));
        foreach (var pool in _pools.Where(p => p.NumberOfRelationshipTemplates > 0))
        {
            foreach (var identity in pool.Identities)
            {
                var sdk = Client.CreateForExistingIdentity(_httpClientPool[Environment.CurrentManagedThreadId], _clientCredentials, identity.UserCredentials);
                for (uint i = 0; i < pool.NumberOfRelationshipTemplates; i++)
                {
                    var relationshipTemplateResponse = await sdk.RelationshipTemplates.CreateTemplate(new CreateRelationshipTemplateRequest
                    {
                        Content = [],
                        ExpiresAt = DateTime.UtcNow.EndOfYear(),
                        MaxNumberOfAllocations = 1000
                    });

                    if (relationshipTemplateResponse.Result is not null)
                        identity.RelationshipTemplates.Add(relationshipTemplateResponse.Result);
                    progress.Increment();
                }
            }
        }

        Console.WriteLine("done.");
    }

    private async Task CreateChallenges()
    {
        Console.Write("Creating Challenges... ");
        using var progress = new ProgressBar(_pools.Sum(p => p.NumberOfChallenges * p.Amount));

        var relevantIdentities = _pools.Where(p => p.NumberOfChallenges > 0).SelectMany(p => p.Identities).ToList();

        var dictionaryOfBags = new ConcurrentDictionary<uint, ConcurrentBag<Challenge>>();

        foreach (var identity in relevantIdentities) dictionaryOfBags.TryAdd(identity.UniqueOrderNumber, []);

        await Parallel.ForEachAsync(relevantIdentities, async (identity, _) =>
        {
            var sdk = Client.CreateForExistingIdentity(_httpClientPool[Environment.CurrentManagedThreadId], _clientCredentials, identity.UserCredentials);
            for (var i = 0; i < identity.Pool.NumberOfChallenges; i++)
            {
                var challenge = (await sdk.Challenges.CreateChallenge()).Result;
                if (challenge is not null) dictionaryOfBags[identity.UniqueOrderNumber].Add(challenge);
                progress.Increment();
            }
        });

        foreach (var (uon, bag) in dictionaryOfBags)
        {
            relevantIdentities.Single(i => i.UniqueOrderNumber == uon).Challenges = [.. bag];
        }

        Console.WriteLine("done.");
    }

    private async Task CreateDatawalletModifications()
    {
        Console.Write("Creating DataWalletModifications... ");
        using var progress = new ProgressBar(_pools.Where(p => p.NumberOfDatawalletModifications > 0).Sum(p => p.Amount));

        await Parallel.ForEachAsync(_pools.Where(p => p.NumberOfDatawalletModifications > 0), async (pool, _) =>
        {
            foreach (var identity in pool.Identities)
            {
                var sdk = Client.CreateForExistingIdentity(_httpClientPool[Environment.CurrentManagedThreadId], _clientCredentials, identity.UserCredentials);
                var startDatawalletVersionUpgradeResponse = await sdk.SyncRuns.StartSyncRun(new StartSyncRunRequest { Type = SyncRunType.DatawalletVersionUpgrade, Duration = 100 }, 1);

                if (startDatawalletVersionUpgradeResponse.Result is null) continue;

                var finalizeDatawalletVersionUpgradeResponse = await sdk.SyncRuns.FinalizeDatawalletVersionUpgrade(startDatawalletVersionUpgradeResponse.Result.SyncRun.Id,
                    new FinalizeDatawalletVersionUpgradeRequest
                    {
                        DatawalletModifications = PreGenerateDatawalletModifications(identity.Pool.NumberOfDatawalletModifications),
                        NewDatawalletVersion = 3
                    });

                if (finalizeDatawalletVersionUpgradeResponse.Result is not null)
                    identity.SetDatawalletModifications(finalizeDatawalletVersionUpgradeResponse.Result.DatawalletModifications);

                progress.Increment();
            }
        });

        Console.WriteLine("done.");
    }


    /// <summary>
    /// Given a number of Datawallet Modifications to generate, distributes them semi-randomly with the following types:
    /// <ol>
    /// <li>of type create: 3/10</li>
    /// <li>of type update: 6/10</li>
    /// <li>of type delete: 1/10</li>
    /// </ol>
    /// </summary>
    /// <see cref="PushDatawalletModificationsRequestItem.Type"/>
    /// <returns></returns>
    private static List<PushDatawalletModificationsRequestItem> PreGenerateDatawalletModifications(uint number)
    {
        var result = new List<PushDatawalletModificationsRequestItem>();
        uint objectIterator = 1;
        if (number < 10)
        {
            // can't be divided properly. Will only do creates.
            for (uint i = 0; i < number; i++)
            {
                result.Add(new PushDatawalletModificationsRequestItem
                {
                    Collection = "Performance-Tests",
                    DatawalletVersion = 2,
                    Type = "Create",
                    ObjectIdentifier = "OBJ" + objectIterator++.ToString("D12"),
                    PayloadCategory = "Userdata"
                });
            }

            return result;
        }

        var idsAndOperationsDictionary = new Dictionary<string, List<string>>();
        var random = new Random();

        for (uint i = 0; i < number; i++)
        {
            if (i < number * 3 / 10)
            {
                // create
                idsAndOperationsDictionary.Add("OBJ" + objectIterator++.ToString("D12"), ["Create"]);
            }
            else if (i < number * 9 / 10)
            {
                // update
                random.GetRandomElement(idsAndOperationsDictionary).Add("Update");
            }
            else
            {
                // delete
                var selectedKey = idsAndOperationsDictionary.Where(p => !p.Value.Contains("Delete")).Select(p => p.Key).First();
                idsAndOperationsDictionary[selectedKey].Add("Delete");
            }
        }

        foreach (var (id, operations) in idsAndOperationsDictionary)
        {
            result.AddRange(operations.Select(operation => new PushDatawalletModificationsRequestItem
            {
                Collection = "Requests",
                DatawalletVersion = 1,
                ObjectIdentifier = id,
                Type = operation,
                PayloadCategory = "Metadata"
            }));
        }

        return result;
    }
}
