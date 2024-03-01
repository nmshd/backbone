using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Messages.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Messages.Application.Messages.DTOs;
using Backbone.Modules.Messages.Application.Messages.Queries.GetMessage;
using Backbone.Modules.Messages.Domain.Entities;
using Backbone.Modules.Messages.Domain.Ids;
using MediatR;
using NSec.Cryptography;

namespace Backbone.Modules.Messages.Application.Messages.Queries.ViewMessages;
public class Handler : IRequestHandler<GetViewMessageQuery, ViewMessageDTO>
{
    private readonly IMessagesRepository _messagesRepository;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;

    public Handler()
    {
    }

    public Handler(IMapper mapper)
    {
        _mapper = mapper;
    }

    public Handler(IUserContext userContext, IMapper mapper, IMessagesRepository messagesRepository)
    {
        _userContext = userContext;
        _mapper = mapper;
        _messagesRepository = messagesRepository;
    }

    public async Task<ViewMessageDTO> Handle(GetViewMessageQuery request, CancellationToken cancellationToken)
    {
        if (request.MessageId == null || request.SymmetricKey == null)
        {
            throw new NullReferenceException(nameof(request));
        }

        var message = await _messagesRepository.Find(request.MessageId, _userContext.GetAddress(), cancellationToken, track: true);

        var decryptedBody = message.Decrypt(request.SymmetricKey);

        var response = new ViewMessageDTO
        {
            Id = message.Id,
            CreatedAt = message.CreatedAt,
            Body = decryptedBody
        };

        return response;
    }
}
