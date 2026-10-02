using MediatR;

namespace Backbone.Modules.Devices.Module.Features.PushNotifications.DeletePnsRegistrationsOfIdentity;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("DeletePnsRegistrationsOfIdentityCommand")]
public class Command : IRequest
{
    public required string IdentityAddress { get; init; }
}
