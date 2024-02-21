using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.Modules.Devices.Application.Identities.Commands.RejectDeletionProcess;
public class RejectDeletionProcessResponse
{
    public RejectDeletionProcessResponse(IdentityDeletionProcess deletionProcess, IdentityAddress identityAddress, DeviceId deviceId)
    {
        Id = deletionProcess.Id;
        Status = deletionProcess.Status;
        CreatedAt = deletionProcess.CreatedAt;
        RejectedByDevice = deviceId;
    }

    public string Id { get; }
    public DeletionProcessStatus Status { get; }
    public DateTime CreatedAt { get; }
    public string RejectedByDevice { get; }
}
