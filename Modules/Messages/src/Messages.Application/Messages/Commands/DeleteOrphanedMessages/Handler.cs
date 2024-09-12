using Backbone.Modules.Messages.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Messages.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Messages.Application.Messages.Commands.DeleteOrphanedMessages;

public class Handler : IRequestHandler<DeleteOrphanedMessagesCommand>
{
    private readonly IMessagesRepository _messagesRepository;
    private readonly ApplicationOptions _applicationOptions;

    public Handler(IMessagesRepository messagesRepository, IOptions<ApplicationOptions> applicationOptions)
    {
        _messagesRepository = messagesRepository;
        _applicationOptions = applicationOptions.Value;
    }

    public async Task Handle(DeleteOrphanedMessagesCommand request, CancellationToken cancellationToken)
    {
        await _messagesRepository.Delete(Message.IsMessageOrphaned(_applicationOptions.DidDomainName), cancellationToken);
    }
}
