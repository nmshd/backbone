using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Clients.ChangeClientSecret;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("ChangeClientSecretCommand")]
public class Command : IRequest<Response>
{
    public required string ClientId { get; set; }

    public required string NewSecret { get; set; }
}
