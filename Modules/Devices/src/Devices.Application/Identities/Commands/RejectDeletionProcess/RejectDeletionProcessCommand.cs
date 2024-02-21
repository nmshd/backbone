using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.RejectDeletionProcess;
public class RejectDeletionProcessCommand : IRequest<RejectDeletionProcessResponse>
{
    public RejectDeletionProcessCommand(string deletionProcessId)
    {
        DeletionProcessId = deletionProcessId;
    }

    public string DeletionProcessId { get; set; }
}
