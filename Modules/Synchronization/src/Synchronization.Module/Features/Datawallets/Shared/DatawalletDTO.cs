using Backbone.Modules.Synchronization.Domain.Entities;

namespace Backbone.Modules.Synchronization.Module.Features.Datawallets.Shared;

public class DatawalletDTO
{
    public DatawalletDTO(Datawallet datawallet)
    {
        Version = datawallet.Version;
    }

    public ushort Version { get; set; }
}
