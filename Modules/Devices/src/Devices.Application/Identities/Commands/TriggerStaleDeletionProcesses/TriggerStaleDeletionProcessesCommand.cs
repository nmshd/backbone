using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.TriggerStaleDeletionProcesses;
public class TriggerStaleDeletionProcessesCommand : IRequest<TriggerStaleDeletionProcessesResponse>
{
    public TriggerStaleDeletionProcessesCommand(string identityAddress, string deletionProcessId)
    {
        IdentityAddress = identityAddress;
        DeletionProcessId = deletionProcessId;
    }

    public string IdentityAddress { get; set; }
    public string DeletionProcessId { get; set; }
}
