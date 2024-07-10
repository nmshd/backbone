using System.Diagnostics;
using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Sdk.Endpoints.Messages.Types.Requests;
using Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates.Types.Requests;
using Backbone.Crypto;
using Backbone.Identity.Pool.Creator.PoolsFile;
using Backbone.Identity.Pool.Creator.PoolsGenerator;
using Backbone.Identity.Pool.Creator.Tools;
using Backbone.Tooling;
using Backbone.Tooling.Extensions;
using Org.BouncyCastle.Tls;

namespace Backbone.Identity.Pool.Creator.EntityCreation;
public class EntityCreator
{
    private readonly string _baseAddress;

    private readonly IList<PoolEntry> _pools;
    private readonly SolutionRepresentation _ram;
    private readonly ClientCredentials _clientCredentials;

    public EntityCreator(string baseAddress, string clientId, string clientSecret, IEnumerable<PoolEntry> pools, SolutionRepresentation ram)
    {
        _baseAddress = baseAddress;
        _clientCredentials = new ClientCredentials(clientId, clientSecret);
        _pools = pools.ToList();
        _ram = ram;
    }

    public async Task StartCreation()
    {
        await CreateIdentities();

        LoadRelationshipsAndMessagesConfiguration();

        await CreateRelationships();
        await CreateMessages();
        await CreateRelationshipTemplates();
    }

    private async Task CreateRelationships()
    {
        Console.Write("Establishing Relationships... ");
        using var progress = new ProgressBar(_ram.GetRelationshipCount());

        var establishedRelationships = new HashSet<string>();
        foreach (var identity in _pools.SelectMany(p => p.Identities))
        {
            foreach (var relatedIdentity in identity.IdentitiesToEstablishRelationshipsWith)
            {
                if (establishedRelationships.Contains(GetRelationshipRepresentation(identity, relatedIdentity))) continue;

                var sdk = Client.CreateForExistingIdentity(_baseAddress, _clientCredentials, identity.UserCredentials);
                var relatedSdk = Client.CreateForExistingIdentity(_baseAddress, _clientCredentials, relatedIdentity.UserCredentials);

                var templateResponse = await sdk.RelationshipTemplates.CreateTemplate(new() { Content = [], ExpiresAt = DateTime.Now.AddYears(2), MaxNumberOfAllocations = 50 });
                var createRelationshipResponse = await relatedSdk.Relationships.CreateRelationship(new() { RelationshipTemplateId = templateResponse.Result!.Id, Content = [] });
                var acceptRelationshipResponse = await sdk.Relationships.AcceptRelationship(createRelationshipResponse.Result!.Id, new());
                var relationshipRepresentation = GetRelationshipRepresentation(identity, relatedIdentity);
                establishedRelationships.Add(relationshipRepresentation);
                progress.Increment();
            }
        }
        Console.WriteLine("done.");
    }

    private static string GetRelationshipRepresentation(Identity identity, Identity relatedIdentity)
    {
        return identity.Uon > relatedIdentity.Uon ? $"{identity.Uon}-{relatedIdentity.Uon}" : $"{relatedIdentity.Uon}-{identity.Uon}";
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
        Console.Write("Sendiong Messages... ");
        using var progress = new ProgressBar(_ram.GetSentMessagesCount());

        foreach (var identity in _pools.SelectMany(p => p.Identities))
        {
            foreach (var recipientIdentity in identity.IdentitiesToSendMessagesTo)
            {
                var sdk = Client.CreateForExistingIdentity(_baseAddress, _clientCredentials, identity.UserCredentials);

                await sdk.Messages.SendMessage(new()
                {
                    Recipients = [new SendMessageRequestRecipientInformation { Address = recipientIdentity.Address, EncryptedKey = ConvertibleString.FromUtf8(new string('A', 152)).BytesRepresentation }],
                    Attachments = [],
                    Body = []
                });
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
                var sdk = await Client.CreateForNewIdentity(_baseAddress, _clientCredentials, PasswordHelper.GeneratePassword(18, 24));
                if (sdk.DeviceData is null)
                    throw new Exception("The SDK could not be used to create a new Identity.");

                var createdIdentity = new Identity(sdk.DeviceData.UserCredentials, sdk.IdentityData?.Address ?? "no address", sdk.DeviceData.DeviceId, pool, i + 1, uon++);

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
        }
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
}
