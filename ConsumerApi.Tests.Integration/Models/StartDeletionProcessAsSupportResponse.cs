using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.ConsumerApi.Tests.Integration.Models;

public class StartDeletionProcessAsSupportResponse
{
    public string Id { get; set; }
    public DeletionProcessStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
