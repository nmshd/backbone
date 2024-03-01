using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backbone.Modules.Messages.Application.Messages.DTOs;
using Backbone.Modules.Messages.Domain.Entities;
using Backbone.Modules.Messages.Domain.Ids;
using MediatR;

namespace Backbone.Modules.Messages.Application.Messages.Queries.ViewMessages;

public class GetViewMessageQuery : IRequest<ViewMessageDTO>
{
    public MessageId MessageId { get; set; }
    public string SymmetricKey { get; set; }
}
