using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Messages.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Messages.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Messages.Application.Messages.Commands.AnonymizeMessagesOfIdentity;

public class Handler : IRequestHandler<AnonymizeMessagesOfIdentityCommand>
{
    private readonly IMessagesRepository _messagesRepository;
    private readonly ApplicationOptions _applicationOptions;

    public Handler(IMessagesRepository messagesRepository, IOptions<ApplicationOptions> applicationOptions)
    {
        _messagesRepository = messagesRepository;
        _applicationOptions = applicationOptions.Value;
    }

    public async Task Handle(AnonymizeMessagesOfIdentityCommand request, CancellationToken cancellationToken)
    {
        var messages = await _messagesRepository.Find(Message.HasParticipant(request.IdentityAddress), cancellationToken);

        var newIdentityAddress = IdentityAddress.GetAnonymized(_applicationOptions.DidDomainName);

        foreach (var message in messages)
        {
            message.AnonymizeParticipant(request.IdentityAddress, newIdentityAddress);
        }

        await _messagesRepository.Update(messages);
    }
}
