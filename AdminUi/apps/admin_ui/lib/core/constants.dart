import 'package:flutter/widgets.dart';

import '/core/core.dart';

class Gaps {
  Gaps._();

  static const SizedBox h4 = SizedBox(height: 4);
  static const SizedBox h8 = SizedBox(height: 8);
  static const SizedBox h16 = SizedBox(height: 16);
  static const SizedBox h24 = SizedBox(height: 24);
  static const SizedBox h32 = SizedBox(height: 32);
  static const SizedBox h40 = SizedBox(height: 40);

  static const SizedBox w4 = SizedBox(width: 4);
  static const SizedBox w8 = SizedBox(width: 8);
  static const SizedBox w16 = SizedBox(width: 16);
  static const SizedBox w24 = SizedBox(width: 24);
  static const SizedBox w32 = SizedBox(width: 32);
  static const SizedBox w40 = SizedBox(width: 40);
}

class MessageTemplate {
  String getMessageForDeletionProcessAuditLog(BuildContext context, String messageKey, Map<String, String> additionalData) {
    final messageTemplates = {
      'StartedByOwner': context.l10n.messageTemplate_startedByOwner,
      'StartedBySupport': context.l10n.messageTemplate_startedBySupport,
      'Approved': context.l10n.messageTemplate_approved,
      'Rejected': context.l10n.messageTemplate_rejected,
      'CancelledByOwner': context.l10n.messageTemplate_cancelledByOwner,
      'CancelledBySupport': context.l10n.messageTemplate_cancelledBySupport,
      'CancelledAutomatically': context.l10n.messageTemplate_cancelledAutomatically,
      'ApprovalReminder1Sent': context.l10n.messageTemplate_approvalReminder1Sent,
      'ApprovalReminder2Sent': context.l10n.messageTemplate_approvalReminder2Sent,
      'ApprovalReminder3Sent': context.l10n.messageTemplate_approvalReminder3Sent,
      'GracePeriodReminder1Sent': context.l10n.messageTemplate_gracePeriodReminder1Sent,
      'GracePeriodReminder2Sent': context.l10n.messageTemplate_gracePeriodReminder2Sent,
      'GracePeriodReminder3Sent': context.l10n.messageTemplate_gracePeriodReminder3Sent,
      'DataDeleted': '${context.l10n.all} {aggregateType} ${context.l10n.messageTemplate_messagesHaveBeenDeleted}',
    };

    var messageTemplate = messageTemplates[messageKey];

    additionalData.forEach((key, value) {
      final placeholder = '{$key}';
      messageTemplate = messageTemplate?.replaceAll(placeholder, value);
    });

    return messageTemplate!;
  }
}
