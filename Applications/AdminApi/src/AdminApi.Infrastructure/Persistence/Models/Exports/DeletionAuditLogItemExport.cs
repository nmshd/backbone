using Backbone.AdminApi.Infrastructure.Persistence.Models.Devices;

namespace Backbone.AdminApi.Infrastructure.Persistence.Models.Exports;

public class DeletionAuditLogItemExport
{
    public required DeletionProcessStatus? OldStatus { get; set; }
    public required DeletionProcessStatus? NewStatus { get; set; }
    public required byte[] IdentityAddressHash { get; set; }
    public required string MessageKey { get; set; }
    public required DateTime CreatedAt { get; set; }
}
