using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Module.Features.Identities.CreateIdentity;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("CreateIdentityResponse")]
public class Response
{
    public Response(Identity identity)
    {
        Address = identity.Address;
        CreatedAt = identity.CreatedAt;
        Device = new CreateIdentityResponseDevice
        {
            Id = identity.Devices.First().Id,
            Username = identity.Devices.First().Username,
            CreatedAt = identity.Devices.First().CreatedAt
        };
    }

    public string Address { get; set; }
    public DateTime CreatedAt { get; set; }
    public CreateIdentityResponseDevice Device { get; set; }
}

public class CreateIdentityResponseDevice
{
    public required string Id { get; set; }
    public required string Username { get; set; }
    public required DateTime CreatedAt { get; set; }
}
