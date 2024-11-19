using Backbone.ConsumerApi.Sdk;
using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Sdk.Endpoints.Challenges.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.SyncRuns.Types;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Enums;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;

public record DomainIdentity(
    UserCredentials UserCredentials,
    IdentityData? IdentityData,
    int ConfigurationIdentityAddress, // the address from pool-config json 
    int NumberOfDevices,
    int NumberOfRelationshipTemplates,
    IdentityPoolType IdentityPoolType,
    int NumberOfChallenges,
    string PoolAlias,
    int NumberOfDatawalletModifications)
{
    public readonly List<string> DeviceIds = [];

    public string? IdentityAddress => IdentityData?.Address; // the real identity address returned by sdk

    public List<RelationshipTemplateBag> RelationshipTemplates { get; } = [];

    public Dictionary<string, DomainIdentity> EstablishedRelationshipsById { get; } = [];
    public List<Challenge> Challenges { get; } = [];

    public List<MessageBag> SentMessages { get; } = [];

    public List<CreatedDatawalletModification> DatawalletModifications { get; private set; } = [];

    public void AddDevice(string deviceId)
    {
        DeviceIds.Add(deviceId);
    }

    public void SetDatawalletModifications(List<CreatedDatawalletModification> resultDatawalletModifications)
    {
        DatawalletModifications = resultDatawalletModifications;
    }
}
