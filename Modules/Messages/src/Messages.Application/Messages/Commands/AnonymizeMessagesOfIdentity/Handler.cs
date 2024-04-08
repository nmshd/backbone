using System.Text;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Messages.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Messages.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Messages.Application.Messages.Commands.AnonymizeMessagesOfIdentity;

public class Handler : IRequestHandler<AnonymizeMessagesOfIdentityCommand>
{
    private const string DELETED_IDENTITY_STRING = "deleted identity";
    private readonly IMessagesRepository _messagesRepository;
    private readonly ApplicationOptions _applicationOptions;

    public Handler(IMessagesRepository messagesRepository, IOptions<ApplicationOptions> applicationOptions)
    {
        _messagesRepository = messagesRepository;
        _applicationOptions = applicationOptions.Value;
    }

    public async Task Handle(AnonymizeMessagesOfIdentityCommand request, CancellationToken cancellationToken)
    {
        var messages = await _messagesRepository.Find(Message.WasCreatedBy(request.IdentityAddress), cancellationToken);

        var newIdentityAddress = IdentityAddress.Create(Encoding.Unicode.GetBytes(DELETED_IDENTITY_STRING), _applicationOptions.InstanceUrl);

        foreach (var message in messages)
        {
            message.ReplaceIdentityAddress(request.IdentityAddress, newIdentityAddress);
        }

        await _messagesRepository.Update(messages);
    }
}
