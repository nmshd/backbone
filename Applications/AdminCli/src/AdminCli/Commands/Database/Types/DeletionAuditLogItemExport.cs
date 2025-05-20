using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.AdminCli.Commands.Database.Types;

public class DeletionAuditLogItemExport
{
    public required DeletionProcessStatus? OldStatus { get; set; }
    public required DeletionProcessStatus? NewStatus { get; set; }
    public required byte[] IdentityAddressHash { get; set; }
    public required MessageKey MessageKey { get; set; }
    public required DateTime CreatedAt { get; set; }
}
