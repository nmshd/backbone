import 'package:admin_api_types/admin_api_types.dart';
import 'package:flutter/material.dart';

import '../../../core/extensions.dart';

class RecipientsButton extends StatelessWidget {
  final List<MessageRecipients> recipients;

  const RecipientsButton({required this.recipients, super.key});

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.all(4),
      child: FilledButton(
        onPressed: () {
          showDialog<Widget>(
            context: context,
            builder: (BuildContext context) {
              return _RecipientsDialog(recipients: recipients);
            },
          );
        },
        child: const Text('...'),
      ),
    );
  }
}

class _RecipientsDialog extends StatelessWidget {
  final List<MessageRecipients> recipients;

  const _RecipientsDialog({required this.recipients});

  @override
  Widget build(BuildContext context) {
    return AlertDialog(
      title: Text(context.l10n.listOfAllRecipients),
      content: Padding(
        padding: const EdgeInsets.all(8),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: recipients
              .map(
                (recipient) => Padding(
                  padding: const EdgeInsets.all(4),
                  child: Text(recipient.address),
                ),
              )
              .toList(),
        ),
      ),
      actions: [
        TextButton(
          onPressed: () => Navigator.of(context).pop(),
          child: Text(context.l10n.close),
        ),
      ],
    );
  }
}
