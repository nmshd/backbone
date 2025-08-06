using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using MediatR;
using Microsoft.Extensions.Options;
using ApplicationException = Backbone.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace Backbone.Modules.Devices.Application.Notifications.Commands.SendNotification;

public class Handler : IRequestHandler<SendNotificationCommand>
{
    private readonly IPushNotificationSender _pushNotificationSender;
    private readonly IRelationshipsRepository _relationshipsRepository;
    private readonly ApplicationConfiguration.NotificationsConfiguration? _notifications;
    private readonly IdentityAddress _activeIdentity;

    public Handler(IPushNotificationSender pushNotificationSender, IRelationshipsRepository relationshipsRepository, IUserContext userContext,
        IOptions<ApplicationConfiguration> applicationConfiguration)
    {
        _pushNotificationSender = pushNotificationSender;
        _relationshipsRepository = relationshipsRepository;
        _activeIdentity = userContext.GetAddress();
        _notifications = applicationConfiguration.Value.Notifications;
    }

    public async Task Handle(SendNotificationCommand request, CancellationToken cancellationToken)
    {
        var parsedRecipients = request.Recipients.Select(IdentityAddress.ParseUnsafe).ToArray();

        await EnsureRelationshipsToAllRecipientsExist(parsedRecipients, cancellationToken);

        var notification = GetNotificationFromConfigurationByCode(request);

        var parsedNotificationTexts = notification.Translations.ToDictionary(kv => kv.Key, kv => new NotificationText(kv.Value.Title, kv.Value.Body));
        var notificationId = $"notification_{request.Code}";
        var pushNotificationFilter = SendPushNotificationFilter.AllDevicesOf(parsedRecipients);

        await _pushNotificationSender.SendNotification(notificationId, parsedNotificationTexts, pushNotificationFilter, cancellationToken);
    }

    private ApplicationConfiguration.NotificationTextsConfiguration GetNotificationFromConfigurationByCode(SendNotificationCommand request)
    {
        var textFromConfiguration = _notifications?.Texts.FirstOrDefault(t => t.Code == request.Code);

        if (textFromConfiguration == null)
            throw new ApplicationException(ApplicationErrors.Notifications.CodeDoesNotExist(_notifications!.Texts.Select(t => t.Code)));

        return textFromConfiguration;
    }

    private async Task EnsureRelationshipsToAllRecipientsExist(IdentityAddress[] recipients, CancellationToken cancellationToken)
    {
        var relationshipsToRecipients = await _relationshipsRepository.GetYoungestRelationships(_activeIdentity, recipients, cancellationToken);

        var recipientsWithoutRelationships = recipients.Where(rec => !relationshipsToRecipients.Any(rel => rel.HasParticipant(rec))).ToList();

        if (recipientsWithoutRelationships.Count != 0)
            throw new ApplicationException(ApplicationErrors.Notifications.NoRelationshipToOneOrMoreRecipientsExists(recipientsWithoutRelationships));

        foreach (var recipient in recipients)
        {
            var relationship = relationshipsToRecipients.First(r => r.HasParticipant(recipient));
            relationship.EnsureSendingNotificationsIsAllowed();
        }
    }
}
