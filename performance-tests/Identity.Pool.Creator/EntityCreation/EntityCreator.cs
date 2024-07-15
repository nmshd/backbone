using System.Collections.Concurrent;
using System.Security.Principal;
using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Sdk.Endpoints.Challenges.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Datawallets.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.Messages.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates.Types.Requests;
using Backbone.Crypto;
using Backbone.Identity.Pool.Creator.Application.Printer;
using Backbone.Identity.Pool.Creator.PoolsFile;
using Backbone.Identity.Pool.Creator.PoolsGenerator;
using Backbone.Identity.Pool.Creator.Tools;
using Backbone.Tooling;
using Backbone.Tooling.Extensions;

namespace Backbone.Identity.Pool.Creator.EntityCreation;
public class EntityCreator
{
    private readonly string _baseAddress;

    private readonly IList<PoolEntry> _pools;
    private readonly SolutionRepresentation _ram;
    private readonly IPrinter _printer;
    private Dictionary<uint, Identity> _identitiesDictionary;
    private readonly ClientCredentials _clientCredentials;

    public EntityCreator(string baseAddress, string clientId, string clientSecret, IEnumerable<PoolEntry> pools, SolutionRepresentation ram, IPrinter printer)
    {
        _baseAddress = baseAddress;
        _clientCredentials = new ClientCredentials(clientId, clientSecret);
        _pools = pools.ToList();
        _ram = ram;
        _printer = printer;
        _identitiesDictionary = [];
    }

    public async Task StartCreation()
    {
        await CreateIdentities();
        LoadRelationshipsAndMessagesConfiguration();
        _identitiesDictionary = _pools.SelectMany(p => p.Identities).ToDictionary(i => i.Uon);

        await CreateRelationships();
        await CreateChallenges();
        await CreateMessages();
        await CreateRelationshipTemplates();
        await CreateDatawalletModifications();


        _printer.OutputAll(_pools);
    }

    private async Task CreateRelationships()
    {
        Console.Write("Establishing Relationships... ");
        using var progress = new ProgressBar(_ram.GetRelationshipCount());

        var establishedRelationships = new HashSet<(uint a, uint b)>();

        foreach (var identity in _pools.SelectMany(p => p.Identities))
        {
            foreach (var relatedIdentity in identity.IdentitiesToEstablishRelationshipsWith)
            {
                establishedRelationships.Add(GetRelationshipRepresentation(identity, relatedIdentity));
            }
        }

        await Parallel.ForEachAsync(establishedRelationships, new ParallelOptions() { MaxDegreeOfParallelism = establishedRelationships.Count < 32 ? 4 : 16 }, async (pair, _) =>
        {
            var sdk = Client.CreateForExistingIdentity(_baseAddress, _clientCredentials, _identitiesDictionary[pair.a].UserCredentials);
            var relatedSdk = Client.CreateForExistingIdentity(_baseAddress, _clientCredentials, _identitiesDictionary[pair.b].UserCredentials);

            var templateResponse = await sdk.RelationshipTemplates.CreateTemplate(new() { Content = [], ExpiresAt = DateTime.Now.AddYears(2), MaxNumberOfAllocations = 50 });
            var createRelationshipResponse = await relatedSdk.Relationships.CreateRelationship(new() { RelationshipTemplateId = templateResponse.Result!.Id, Content = [] });
            var acceptRelationshipResponse = await sdk.Relationships.AcceptRelationship(createRelationshipResponse.Result!.Id, new());
            if(acceptRelationshipResponse is not null && acceptRelationshipResponse.Result is not null) 
                _identitiesDictionary[pair.a].EstablishedRelationshipsById.Add(acceptRelationshipResponse.Result.Id, _identitiesDictionary[pair.b]);

            progress.Increment();
        });

        Console.WriteLine("done.");
    }

    private static (uint a, uint b) GetRelationshipRepresentation(Identity identity, Identity relatedIdentity)
    {
        return identity.Uon > relatedIdentity.Uon ? (identity.Uon, relatedIdentity.Uon) : (relatedIdentity.Uon, identity.Uon);
    }

    private void LoadRelationshipsAndMessagesConfiguration()
    {
        var dict = _pools.SelectMany(p => p.Identities).ToDictionary(i => i.Uon);

        foreach (var pool in _pools)
        {
            foreach (var identity in pool.Identities)
            {
                var res = _ram.GetRelationshipsAndMessageSentCountByIdentity(identity.Uon);
                foreach (var (relatedIdentity, messageCount) in res)
                {
                    var success = identity.AddIdentityToEstablishRelationshipsWith(dict[relatedIdentity], skipCapacityCheck: true);
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
                var sdk = Client.CreateForExistingIdentity(_baseAddress, _clientCredentials, identity.UserCredentials);

                var messageResponse = await sdk.Messages.SendMessage(new()
                {
                    Recipients = [new SendMessageRequestRecipientInformation { Address = recipientIdentity.Address, EncryptedKey = ConvertibleString.FromUtf8(new string('A', 152)).BytesRepresentation }],
                    Attachments = [],
                    Body = []
                });
                if(messageResponse is not null && messageResponse.Result is not null)
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

        uint uon = 1;

        foreach (var pool in _pools)
        {
            for (uint i = 0; i < pool.Amount; i++)
            {
                pool.IdentityUons.Enqueue(uon++);
            }
        }

        await Parallel.ForEachAsync(_pools, async (pool, _) =>
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
                var sdk = Client.CreateForExistingIdentity(_baseAddress, _clientCredentials, identity.UserCredentials);
                for (uint i = 0; i < pool.NumberOfRelationshipTemplates - identity.IdentitiesToEstablishRelationshipsWith.Count; i++)
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
        Console.WriteLine("done.");
    }

    private async Task CreateChallenges()
    {
        Console.Write("Creating Challenges... ");
        using var progress = new ProgressBar(_pools.Sum(p => p.NumberOfChallenges * p.Amount));

        var relevantIdentities = _pools.Where(p => p.NumberOfChallenges > 0).SelectMany(p => p.Identities);

        var dictionaryOfBags = new ConcurrentDictionary<uint, ConcurrentBag<Challenge>>();

        foreach (var identity in relevantIdentities) dictionaryOfBags.TryAdd(identity.Uon, []);

        await Parallel.ForEachAsync(relevantIdentities, async (identity, _) =>
        {
            var sdk = Client.CreateForExistingIdentity(_baseAddress, _clientCredentials, identity.UserCredentials);
            for (var i = 0; i < identity.Pool.NumberOfChallenges; i++)
            {
                var challenge = (await sdk.Challenges.CreateChallenge()).Result;
                if (challenge is not null) dictionaryOfBags[identity.Uon].Add(challenge);
                progress.Increment();
            }
        });

        foreach (var (uon, bag) in dictionaryOfBags)
        {
            relevantIdentities.Single(i => i.Uon == uon).Challenges = [.. bag];
        }
        Console.WriteLine("done.");
    }

    private async Task CreateDatawalletModifications()
    {
        Console.Write("Creating DataWalletModifications... ");
        using var progress = new ProgressBar(_pools.Where(p => p.NumberOfDatawalletModifications > 0).Sum(p => p.Amount));

        await Parallel.ForEachAsync(_pools.Where(p => p.NumberOfDatawalletModifications > 0), async (pool, ct) =>
        {
            foreach (var identity in pool.Identities)
            {
                var sdk = Client.CreateForExistingIdentity(_baseAddress, _clientCredentials, identity.UserCredentials);
                {
                    var request = new PushDatawalletModificationsRequest()
                    {
                        LocalIndex = 0,
                        Modifications = []
                    };

                    for (uint i = 0; i < pool.NumberOfDatawalletModifications; i++)
                    {
                        request.Modifications.Add(new()
                        {
                            Collection = "Requests",
                            DatawalletVersion = 1,
                            ObjectIdentifier = identity.Address,
                            Type = "Create",
                            PayloadCategory = "MetaData"
                        });
                    }
                    var ret = await sdk.Datawallet.PushDatawalletModifications(request, supportedDatawalletVersion: 0);
                    identity.PushDatawalletModificationsResponse = ret.Result;
                    progress.Increment();
                }
            }
        });

        Console.WriteLine("done.");
    }
}
