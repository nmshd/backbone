using Backbone.Modules.Synchronization.Application.Datawallets.DTOs;
using MediatR;

namespace Backbone.Modules.Synchronization.Application.Datawallets.Commands.PushDatawalletModifications;

public class PushDatawalletModificationsCommand : IRequest<PushDatawalletModificationsResponse>
{
    public long? LocalIndex { get; init; }
    public required ushort SupportedDatawalletVersion { get; init; }
    public required PushDatawalletModificationItem[] Modifications { get; init; }
}
