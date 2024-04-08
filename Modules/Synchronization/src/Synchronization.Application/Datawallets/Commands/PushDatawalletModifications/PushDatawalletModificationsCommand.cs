using System.Text.Json.Serialization;
using Backbone.Modules.Synchronization.Application.Datawallets.DTOs;
using MediatR;

namespace Backbone.Modules.Synchronization.Application.Datawallets.Commands.PushDatawalletModifications;

public class PushDatawalletModificationsCommand : IRequest<PushDatawalletModificationsResponse>
{
    public PushDatawalletModificationsCommand(PushDatawalletModificationItem[] modifications, ushort supportedDatawalletVersion) : this(modifications, null, supportedDatawalletVersion) { }

    [JsonConstructor]
    public PushDatawalletModificationsCommand(PushDatawalletModificationItem[] modifications, long? localIndex, ushort supportedDatawalletVersion)
    {
        LocalIndex = localIndex;
        SupportedDatawalletVersion = supportedDatawalletVersion;
        Modifications = modifications;
    }

    public long? LocalIndex { get; set; }
    public ushort SupportedDatawalletVersion { get; set; }
    public PushDatawalletModificationItem[] Modifications { get; set; }
}
