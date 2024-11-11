using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Sdk.Endpoints.Challenges.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates.Types.Responses;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Enums;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;

public record DomainIdentity(
    UserCredentials UserCredentials,
    string IdentityAddress, // the real identity address returned by sdk 
    string DeviceId,
    IdentityPoolConfiguration IdentityPoolConfiguration,
    int IdentityConfigurationAddress, // the address from pool-config json 
    int NumberOfDevices,
    int NumberOfRelationshipTemplates,
    IdentityPoolType IdentityPoolType,
    int NumberOfChallenges)
{
    public List<string> DeviceIds = [];

    public List<CreateRelationshipTemplateResponse> RelationshipTemplates { get; } = [];

    public Dictionary<string, DomainIdentity> EstablishedRelationshipsById { get; } = [];
    public List<Challenge> Challenges { get; set; } = [];

    public void AddDevice(string deviceId)
    {
        DeviceIds.Add(deviceId);
    }
}
