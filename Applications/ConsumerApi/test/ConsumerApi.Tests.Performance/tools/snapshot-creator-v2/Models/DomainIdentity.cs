using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates.Types.Responses;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Enums;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Models;

public record DomainIdentity(
    UserCredentials UserCredentials,
    string IdentityAddress, // the real identity address returned by sdk 
    string DeviceId,
    IdentityPoolConfiguration IdentityPoolConfiguration,
    int IdentityConfigurationAddress, // the address from pool-config json 
    int NumberOfDevices,
    int NumberOfRelationshipTemplates,
    IdentityPoolType IdentityPoolType
)
{
    public List<string> DeviceIds = [];

    public List<CreateRelationshipTemplateResponse> RelationshipTemplates { get; } = [];

    public Dictionary<string, DomainIdentity> EstablishedRelationshipsById { get; } = [];

    public void AddDevice(string deviceId)
    {
        DeviceIds.Add(deviceId);
    }
}
