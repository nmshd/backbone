using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs.Models;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.CancelDeletionProcessDuringGracePeriod;

public class CancelDeletionProcessDuringGracePeriodCommand : IRequest
{
    public CancelDeletionProcessDuringGracePeriodCommand(string id)
    {
        CancelProcessId = id;
    }

    public string CancelProcessId { get; set; }
}
