import 'package:admin_api_types/admin_api_types.dart';
import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import '/core/extensions.dart';

Future<Tier?> showAllRecipientsDialog({required BuildContext context, required List<MessageRecipient> recipients}) => showDialog<Tier>(
      context: context,
      builder: (BuildContext context) => _ShowAllRecipientsDialog(recipients: recipients),
    );

class _ShowAllRecipientsDialog extends StatefulWidget {
  final List<MessageRecipient> recipients;
  const _ShowAllRecipientsDialog({required this.recipients});

  @override
  State<_ShowAllRecipientsDialog> createState() => _ShowAllRecipientsDialogState();
}

class _ShowAllRecipientsDialogState extends State<_ShowAllRecipientsDialog> {
  @override
  Widget build(BuildContext context) {
    return AlertDialog(
      title: Text(context.l10n.recipientsDialog_listOfAllRecipients, textAlign: TextAlign.center),
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(8)),
      contentPadding: const EdgeInsets.only(left: 24, right: 24, top: 20, bottom: 32),
      content: Padding(
        padding: const EdgeInsets.all(8),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: widget.recipients
              .map(
                (recipient) => Padding(
                  padding: const EdgeInsets.all(4),
                  child: InkWell(
                    onTap: () {
                      context.push('/identities/${recipient.address}');
                      Navigator.of(context).pop();
                    },
                    child: Text(recipient.address),
                  ),
                ),
              )
              .toList(),
        ),
      ),
      actions: [
        OutlinedButton(
          onPressed: () => Navigator.of(context).pop(),
          child: Text(context.l10n.close),
        ),
      ],
    );
  }
}
