using Backbone.Modules.Synchronization.Module.Features.Datawallets.Shared;
using MediatR;

namespace Backbone.Modules.Synchronization.Module.Features.Datawallets.PushDatawalletModifications;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("PushDatawalletModificationsCommand")]
public class Command : IRequest<Response>
{
    public long? LocalIndex { get; init; }
    public required ushort SupportedDatawalletVersion { get; init; }
    public required PushDatawalletModificationItem[] Modifications { get; init; }
}
